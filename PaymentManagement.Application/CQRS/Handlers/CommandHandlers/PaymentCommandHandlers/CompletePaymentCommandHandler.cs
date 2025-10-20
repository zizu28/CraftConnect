using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PayStack.Net;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class CompletePaymentCommandHandler(
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker,
		ILoggingService<CompletePaymentCommandHandler> logger,
		IBackgroundJobService backgroundJob) : IRequestHandler<CompletePaymentCommand, Unit>
	{
		public async Task<Unit> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
		{
			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found.", request.PaymentId);
				throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");
			}
			try
			{
				payment.Complete(request.externalTransactionId);
				var domainEvents = payment.DomainEvents.ToList();
				var completedEvent = domainEvents
					.OfType<PaymentCompletedIntegrationEvent>()
					.FirstOrDefault();
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(payment, cancellationToken);
					await messageBroker.PublishAsync(completedEvent!, cancellationToken);
					payment.ClearEvents();
				}, cancellationToken);
				backgroundJob.Enqueue<IGmailService>(
					"CompletePayment",
					payment => payment.SendEmailAsync(
						request.RecipientEmail,
						"PAYMENT COMPLETE",
						$"Payment with ID {request.PaymentId} to recipient with email {request.RecipientEmail} has been completed.",
						false,
						CancellationToken.None));
				return Unit.Value;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error completing payment with ID {PaymentId}.", request.PaymentId);
				throw;
			}
		}
	}
}
