using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class DeletePaymentCommandHandler(
		IUnitOfWork unitOfWork,
		ILoggingService<DeletePaymentCommandHandler> logger,
		IPaymentRepository paymentRepository,
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker) : IRequestHandler<DeletePaymentCommand, Unit>
	{
		public async Task<Unit> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
		{
			var existingPayment = await paymentRepository.GetByIdAsync(request.Id, cancellationToken);
			if (existingPayment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found.", request.Id);
				throw new KeyNotFoundException($"Payment with ID {request.Id} not found.");
			}
			try
			{
				existingPayment.Cancel(request.Reason);
				var domainEvents = existingPayment.DomainEvents.ToList();
				var cancelledEvent = domainEvents
					.OfType<PaymentCancelledIntegrationEvent>()
					.FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.DeleteAsync(existingPayment.Id, cancellationToken);
					await messageBroker.PublishAsync(cancelledEvent!, cancellationToken);
					existingPayment.ClearEvents();
				}, cancellationToken);
				backgroundJob.Enqueue<IGmailService>(
					"PaymentDeleted",
					emailService => emailService.SendEmailAsync(
						request.Email,
						"PAYMENT DELETED",
						$"Payment with ID {request.Id} has been deleted.",
						false,
						CancellationToken.None));
				logger.LogInformation("Payment with ID {PaymentId} deleted successfully.", request.Id);
				return Unit.Value;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error deleting payment with ID {PaymentId}.", request.Id);
				throw;
			}
		}
	}
}
