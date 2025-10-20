using AutoMapper;
using Core.EventServices;
using Core.Logging;
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
using PaymentManagement.Application.DTOs.PaymentDTOs;
using PaymentManagement.Application.Validators.PaymentValidators;
using PaymentManagement.Domain.Entities;
using PayStack.Net;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class CreatePaymentCommandHandler(
		IPaymentRepository paymentRepository,
		IMapper mapper,
		ILoggingService<CreatePaymentCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork,
		IConfiguration configuration,
		IHttpClientFactory? httpClient) : IRequestHandler<CreatePaymentCommand, PaymentResponseDTO>
	{
		private readonly PayStackApi payStack = new(configuration["Paystack:SecretKey"]);
		public async Task<PaymentResponseDTO> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
		{
			var paystackHttpClient = httpClient?.CreateClient("PaystackClient");
			var response = new PaymentResponseDTO();
			var validator = new PaymentCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.PaymentDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Validation errors occurred.";
				response.Errors = [..validationResult.Errors.Select(e => e.ErrorMessage)];
			}
			try
			{
				TransactionInitializeRequest paystackRequest = new()
				{
					AmountInKobo = (int)(request.PaymentDTO.Amount * 100),
					Email = request.RecipientEmail,
					Currency = request.PaymentDTO.Currency,
					TransactionCharge = (int)(request.TransactionCharge * 100),
				};

				logger.LogInformation($"Request to PayStack:: {JsonConvert.SerializeObject(paystackRequest)}");
				TransactionInitializeResponse paystackResponse = payStack.Transactions.Initialize(paystackRequest);
				logger.LogInformation($"Response from PayStack:: {JsonConvert.SerializeObject(paystackResponse)}");
				if (paystackResponse.Status)
				{
					var paymentEntity = request.PaymentDTO.PaymentType.ToLower() switch
					{
						"booking" => Payment.CreateForBooking(
							request.PaymentDTO.BookingId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
							request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
							Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
								request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
							paystackResponse.Data.Reference, request.PaymentDTO.PaymentType, request.PaymentDTO.Description),

						"invoice" => Payment.CreateForInvoice(
							request.PaymentDTO.InvoiceId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
							request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
							Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
								request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
							paystackResponse.Data.Reference, request.PaymentDTO.PaymentType, request.PaymentDTO.Description),

						_ => Payment.CreateForOrder(
							request.PaymentDTO.OrderId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
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
						var httpResponse = await paystackHttpClient!.GetAsync(paystackResponse.Data.AuthorizationUrl,HttpCompletionOption.ResponseHeadersRead, cancellationToken);

						await paymentRepository.AddAsync(paymentEntity, cancellationToken);
						await messageBroker.PublishAsync(initiatedEvent!, cancellationToken);
						paymentEntity.ClearEvents();
					}, cancellationToken);

					backgroundJob.Enqueue<IGmailService>(
						"PaymentInitiated",
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
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while creating payment.");
				response.IsSuccess = false;
				response.Message = "An error occurred while processing your request.";
				response.Errors = [ex.Message];
			}
			return response;
		}
	}
}
