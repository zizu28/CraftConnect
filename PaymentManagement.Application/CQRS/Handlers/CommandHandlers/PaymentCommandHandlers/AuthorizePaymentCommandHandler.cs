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
	public class AuthorizePaymentCommandHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<AuthorizePaymentCommandHandler> logger,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IConfiguration config) : IRequestHandler<AuthorizePaymentCommand>
	{
		private readonly PayStackApi payStackApi = new(config["Paystack:SecretKey"]);
		public async Task Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request, nameof(request));
			logger.LogInformation("Starting authorization for payment with ID {PaymentId}", request.PaymentId);
			var paymentId = request.PaymentId;
			var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found", paymentId);
				throw new InvalidOperationException($"Payment with ID {paymentId} not found");
			}

			try
			{
				CheckAuthorizationResponse authResponse = payStackApi.Transactions.CheckAuthorization(request.AuthorizationCode, request.RecipientEmail, (int)request.Amount);
				if (!authResponse.Status || authResponse.Data == null)
				{
					logger.LogWarning("Authorization failed for payment {PaymentId}: {Message}", paymentId, authResponse.Message);
					throw new InvalidOperationException($"Authorization failed: {authResponse.Message}");
				}

				payment!.Authorize(request.CorrelationId, request.ExternalTransactionId, request.PaymentIntentId);
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
					"default",
					paymentEmail => paymentEmail.SendEmailAsync(
						request.RecipientEmail,
						"PAYMENT AUTHORIZATION",
						$"Payment with ID {paymentId} has been authorized",
						false,
						CancellationToken.None));
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error authorizing payment {PaymentId}: {ErrorMessage}", paymentId, ex.Message);
				throw;
			}
		}
	}
}
