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
			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found.", request.PaymentId);
				return false;
			}
			if (payment.Status != PaymentStatus.Pending && payment.Status != PaymentStatus.Authorized)
			{
				logger.LogWarning("Cannot fail payment with ID {PaymentId} in {Status} status.", request.PaymentId, payment.Status);
				return false;
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
					await messageBroker.PublishAsync(paymentFailedEvent!, cancellationToken);
					payment.ClearEvents();
				}, cancellationToken);

				backgroundJob.Enqueue<IGmailService>(
					"FailedPayment", 
					failed => failed.SendEmailAsync(
						request.RecipientEmail,
						"PAYMENT FAILED",
						$"Payment with ID {request.PaymentId} has failed.",
						false,
						CancellationToken.None));
				logger.LogInformation("Payment with ID {PaymentId} failed successfully.", request.PaymentId);
				return true;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error failing payment with ID {PaymentId}.", request.PaymentId);
				return false;
			}
		}
	}
}
