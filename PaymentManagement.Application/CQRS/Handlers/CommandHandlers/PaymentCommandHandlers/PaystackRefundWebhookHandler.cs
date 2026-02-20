using Core.Logging;
using Core.SharedKernel.Enums;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class PaystackRefundWebhookHandler(
		IRefundRepository refundRepository,
		IUnitOfWork unitOfWork,
		IBackgroundJobService backgroundJob,
		ILoggingService<PaystackRefundWebhookHandler> logger) : IRequestHandler<ProcessPaystackWebhookCommand, Unit>
	{
		public async Task<Unit> Handle(ProcessPaystackWebhookCommand request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request);
			logger.LogInformation("Processing Paystack webhook for refund {RefundId}, Status: {Status}", request.Data.Id, request.Data.Status);

			try
			{
				var refund = await refundRepository.GetByExternalIdAsync(request.Data.Id.ToString());
				if (refund!.Status != RefundStatus.Pending)
				{
					logger.LogWarning("Refund {RefundId} already processed with status {Status}", refund.Id, refund.Status);
					return Unit.Value;
				}

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					if (request.Data.Status == "success")
					{
						refund.Complete(request.Data.Id.ToString());
						logger.LogInformation("Refund {RefundId} completed successfully", refund.Id);
						backgroundJob.Enqueue<IGmailService>(
							"default",
							emailService => emailService.SendEmailAsync(
								request.RecipientEmail,
								"REFUND COMPLETED",
								$"Your refund of {refund.Amount.Currency}{refund.Amount.Amount} has been processed successfully.",
								false,
								CancellationToken.None));
					}
					else if (request.Data.Status == "failed")
					{
						refund.Fail("Refund failed at payment gateway");
						logger.LogWarning("Refund {RefundId} failed at payment gateway", refund.Id);
						backgroundJob.Enqueue<IGmailService>(
							"default",
							emailService => emailService.SendEmailAsync(
								request.RecipientEmail,
								"REFUND FAILED",
								$"We were unable to process your refund. Please contact support.",
								false,
								CancellationToken.None));
					}
					else
					{
						logger.LogInformation("Refund {RefundId} status: {Status} - no action taken",
							refund.Id, request.Data.Status);
					}
				}, cancellationToken);				
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing Paystack refund webhook for RefundId: {RefundId}", request.Data.Id);
				throw;
			}
			return Unit.Value;
		}
	}
}
