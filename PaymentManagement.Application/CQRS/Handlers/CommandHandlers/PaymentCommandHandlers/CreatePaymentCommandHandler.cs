using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.DTOs.PaymentDTOs;
using PaymentManagement.Application.Validators.PaymentValidators;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class CreatePaymentCommandHandler(
		IPaymentRepository paymentRepository,
		IMapper mapper,
		ILoggingService<CreatePaymentCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<CreatePaymentCommand, PaymentResponseDTO>
	{
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
			}
			try
			{
				var paymentEntity = request.PaymentDTO.PaymentType.ToLower() switch
				{
					"booking" => Payment.CreateForBooking(
						request.PaymentDTO.BookingId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
						request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
						Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
							request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
						request.PaymentDTO.PaymentType, request.PaymentDTO.Description),
					
					"invoice" => Payment.CreateForInvoice(
						request.PaymentDTO.InvoiceId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
						request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
						Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
							request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
						request.PaymentDTO.PaymentType, request.PaymentDTO.Description),
					
					_ => Payment.CreateForOrder(
						request.PaymentDTO.OrderId, new Money(request.PaymentDTO.Amount, request.PaymentDTO.Currency),
						request.PaymentDTO.PayerId, request.PaymentDTO.RecipientId,
						Enum.Parse<PaymentMethod>(request.PaymentDTO.PaymentMethod, true), new Address(request.PaymentDTO.BillingStreet,
							request.PaymentDTO.BillingCity, request.PaymentDTO.BillingPostalCode),
						request.PaymentDTO.PaymentType, request.PaymentDTO.Description),
				};

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.AddAsync(paymentEntity, cancellationToken);
					await messageBroker.PublishAsync(new PaymentInitiatedIntegrationEvent(
						paymentEntity.Id, paymentEntity.InvoiceId, paymentEntity.Amount,
						paymentEntity.PayerId, paymentEntity.RecipientId), cancellationToken);
				}, cancellationToken);

				backgroundJob.Enqueue<IEmailService>(
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
