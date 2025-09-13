using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.IntegrationEvents.RefundIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class RefundPaymentCommandHandler(
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<RefundPaymentCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<RefundPaymentCommand, Unit>
	{
		public async Task<Unit> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
		{
			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found for refund.", request.PaymentId);
				throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");
			}
			try
			{
				payment.ProcessRefund(new Money(request.RefundAmount, request.Currency), request.Reason, request.InitiatedBy);
				var domainEvents = payment.DomainEvents.ToList();
				var refundEvent = domainEvents.OfType<RefundProcessedIntegrationEvent>().FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(payment, cancellationToken);
					await messageBroker.PublishAsync(refundEvent!, cancellationToken);
					payment.ClearEvents();
				}, cancellationToken);
				backgroundJob.Enqueue<IGmailService>(
					"RefundPayment",
					payment => payment.SendEmailAsync(
						request.RecipientEmail,
						"PAYMENT REFUND",
						$"Payment with ID {request.PaymentId} has been refunded to recipient with email {request.RecipientEmail}.",
						false,
						CancellationToken.None));
				return Unit.Value;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing refund for Payment ID {PaymentId}", request.PaymentId);
				throw;
			}
		}
	}
}
