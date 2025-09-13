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
	public class AuthorizePaymentCommandHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<AuthorizePaymentCommandHandler> logger,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<AuthorizePaymentCommand, bool>
	{
		public async Task<bool> Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request, nameof(request));
			logger.LogInformation("Starting authorization for payment {PaymentId}", request.PaymentId);
			var paymentId = request.PaymentId;
			var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment {PaymentId} not found", paymentId);
				return false;
			}
			try
			{
				payment.Authorize(request.ExternalTransactionId, request.PaymentIntentId);
				var domainEvents = payment.DomainEvents.ToList();
				var authorizedEvent = domainEvents
					.OfType<PaymentAuthorizedIntegrationEvent>()
					.FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(payment);
					await messageBroker.PublishAsync(authorizedEvent!, cancellationToken);
					payment.ClearEvents();
				}, cancellationToken);
				logger.LogInformation("Payment {PaymentId} authorized successfully", paymentId);
				backgroundJob.Enqueue<IGmailService>(
					"AuthorizePayment",
					payment => payment.SendEmailAsync(
						request.RecipientEmail,
						"PAYMENT AUTHORIZATION",
						$"Payment with ID {paymentId} has been authorized",
						false,
						CancellationToken.None));
				return true;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error authorizing payment {PaymentId}: {ErrorMessage}", paymentId, ex.Message);
				return false;
			}
		}
	}
}
