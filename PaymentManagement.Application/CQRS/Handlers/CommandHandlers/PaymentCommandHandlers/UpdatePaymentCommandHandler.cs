using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.DTOs.PaymentDTOs;
using PaymentManagement.Application.Validators.PaymentValidators;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class UpdatePaymentCommandHandler(
		IMapper mapper,
		IPaymentRepository paymentRepository,
		ILoggingService<UpdatePaymentCommandHandler> logger,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker) : IRequestHandler<UpdatePaymentCommand, PaymentResponseDTO>
	{
		public async Task<PaymentResponseDTO> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
		{
			var response = new PaymentResponseDTO();
			var validator = new PaymentUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.PaymentDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Validation errors occurred.";
				response.Errors = [..validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogWarning("Validation failed for UpdatePaymentCommand: {Errors}", response.Errors);
				return response;
			}
			var existingPayment = await paymentRepository.GetByIdAsync(request.PaymentDTO.PaymentId, cancellationToken);
			if (existingPayment == null)
			{
				response.IsSuccess = false;
				response.Message = $"Payment with ID {request.PaymentDTO.PaymentId} not found.";
				logger.LogWarning("Payment with ID {PaymentId} not found.", request.PaymentDTO.PaymentId);
				return response;
			}
			try
			{
				mapper.Map(request.PaymentDTO, existingPayment);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(existingPayment, cancellationToken);
					await messageBroker.PublishAsync(new PaymentUpdatedIntegrationEvent(
						request.PaymentDTO.PaymentId, request.PaymentDTO.Description,
						new Address(request.PaymentDTO.BillingStreet!, request.PaymentDTO.BillingCity!, request.PaymentDTO.BillingPostalCode!),
						Enum.Parse<PaymentStatus>(request.PaymentDTO.Status!, true), request.PaymentDTO.ExternalTransactionId!,
						request.PaymentDTO.PaymentIntentId!, request.PaymentDTO.FailureReason!), cancellationToken);
				}, cancellationToken);
				response = mapper.Map<PaymentResponseDTO>(existingPayment);
				response.IsSuccess = true;
				response.Message = "Payment updated successfully.";
				logger.LogInformation("Payment with ID {PaymentId} updated successfully.", existingPayment.Id);
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = "An error occurred while updating the payment.";
				logger.LogError(ex, "Error updating payment with ID {PaymentId}.", existingPayment.Id);
				return response;
			}
		}
	}
}
