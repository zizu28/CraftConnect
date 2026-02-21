using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.Validators.PaymentValidators;
using PaymentManagement.Domain.Entities;
using PayStack.Net;
using System.Security.Cryptography;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class CreatePaymentCommandHandler(
		IPaymentRepository paymentRepository,
		IMapper mapper,
		ILoggingService<CreatePaymentCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork,
		IConfiguration configuration) : IRequestHandler<CreatePaymentCommand, PaymentResponseDTO>
	{
		private readonly PayStackApi payStack = new(configuration["Paystack:SecretKey"]);
		public async Task<PaymentResponseDTO> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
		{
			var response = new PaymentResponseDTO();
			var validator = new PaymentCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.PaymentDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Validation errors occurred.";
				response.Errors = [..validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}

			try
			{
				TransactionInitializeRequest paystackRequest = new()
				{
					AmountInKobo = (int)(request.PaymentDTO.Amount * 100),
					Email = request.RecipientEmail,
					Currency = request.PaymentDTO.Currency,
					TransactionCharge = (int)(request.TransactionCharge * 100),
					Reference = GenerateReference(),
					CallbackUrl = request.CallbackUrl,
				};

				logger.LogInformation($"Request to PayStack:: {JsonConvert.SerializeObject(paystackRequest)}");
				TransactionInitializeResponse paystackResponse = payStack.Transactions.Initialize(paystackRequest, true);
				logger.LogInformation($"Response from PayStack:: {JsonConvert.SerializeObject(paystackResponse)}");
				if (paystackResponse.Status)
				{
					var paymentEntity = request.PaymentDTO.PaymentType.ToLower() switch
					{
						"booking" => Payment.CreateForBooking(
							request.CorrelationId, request.PaymentDTO.BookingId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
							request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
							Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
								request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
							paystackResponse.Data.Reference, request.PaymentDTO.PaymentType, request.PaymentDTO.Description),

						"invoice" => Payment.CreateForInvoice(
							request.CorrelationId, request.PaymentDTO.InvoiceId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
							request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
							Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
								request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
							paystackResponse.Data.Reference, request.PaymentDTO.PaymentType, request.PaymentDTO.Description),

						_ => Payment.CreateForOrder(
							request.CorrelationId, request.PaymentDTO.OrderId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
							request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
							Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
								request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
							paystackResponse.Data.Reference, request.PaymentDTO.PaymentType, request.PaymentDTO.Description),
					};

					var domainEvents = paymentEntity.DomainEvents.ToList();
					var initiatedEvent = domainEvents
						.OfType<PaymentInitiatedIntegrationEvent>()
						.FirstOrDefault();

					await unitOfWork.ExecuteInTransactionAsync(async () =>
					{
						await paymentRepository.AddAsync(paymentEntity, cancellationToken);
						if (initiatedEvent != null)
							await messageBroker.PublishAsync(initiatedEvent!, cancellationToken);
						paymentEntity.ClearEvents();
					}, cancellationToken);

					backgroundJob.Enqueue<IGmailService>(
						"default",
						payment => payment.SendEmailAsync(
							request.RecipientEmail,
							"PAYMENT INITIATED",
							$"Payment with ID {paymentEntity.Id} has been initiated from payer with" +
							$" ID {paymentEntity.PayerId} to recipient with ID {paymentEntity.RecipientId}",
							false,
							CancellationToken.None));
					response = mapper.Map<PaymentResponseDTO>(paymentEntity);
					response.IsSuccess = true;
					response.Message = "Payment created successfully.";
					response.AuthorizationUrl = paystackResponse.Data.AuthorizationUrl;
					response.Reference = paystackResponse.Data.Reference;
				}
				else
				{
					logger.LogWarning("Paystack initialization failed: {Message}", paystackResponse.Message);
					response.IsSuccess = false;
					response.Message = "Payment gateway initialization failed.";
					response.Errors = [paystackResponse.Message ?? "Unknown error from payment gateway"];
					return response;
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while creating payment.");
				response.IsSuccess = false;
				response.Message = "An error occurred while processing your request.";
				response.Errors = [ex.Message];
				throw;
			}
			return response;
		}

		private static string GenerateReference()
		{
			var random = RandomNumberGenerator.GetHexString(16);
			return $"PAY-{random.ToUpper()}";
		}
	}
}
