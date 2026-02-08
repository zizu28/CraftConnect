using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class FailedPaymentCommandHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<FailedPaymentCommandHandler> logger,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<FailedPaymentCommand, bool>
	{
		public async Task<bool> Handle(FailedPaymentCommand request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);
			ArgumentException.ThrowIfNullOrEmpty(request.RecipientEmail, nameof(request.RecipientEmail));
			ArgumentException.ThrowIfNullOrEmpty(request.FailureReason, nameof(request.FailureReason));

			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found.", request.PaymentId);
				throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");
			}

			try
			{
				payment.Fail(request.FailureReason);
				var domainEvents = payment.DomainEvents.ToList();
				var paymentFailedEvent = domainEvents
					.OfType<PaymentFailedIntegrationEvent>()
					.FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(payment, cancellationToken);
					if(paymentFailedEvent != null)
						await messageBroker.PublishAsync(paymentFailedEvent!, cancellationToken);
					payment.ClearEvents();
				}, cancellationToken);

				backgroundJob.Enqueue<IGmailService>(
					"default", 
					failed => failed.SendEmailAsync(
						request.RecipientEmail,
						"PAYMENT FAILED",
						$"Payment with ID {request.PaymentId} failed. Reason: {request.FailureReason}",
						false,
						CancellationToken.None));
				logger.LogInformation("Payment with ID {PaymentId} failed successfully.", request.PaymentId);
				return true;
			}
			catch (InvalidOperationException ex)
			{
				// Business rule violation - return false
				logger.LogWarning("Cannot fail payment {PaymentId}", request.PaymentId);
				return false;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error failing payment with ID {PaymentId}.", request.PaymentId);
				throw;
			}
		}
	}
}
