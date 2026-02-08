using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.RefundIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PayStack.Net;
using System.Net.Http.Json;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class RefundPaymentCommandHandler(
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<RefundPaymentCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IHttpClientFactory httpClientFactory) : IRequestHandler<RefundPaymentCommand, Unit>
	{
		public async Task<Unit> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(request, nameof(request));
			ArgumentException.ThrowIfNullOrEmpty(request.RecipientEmail, nameof(request.RecipientEmail));
			if (request.PaymentId == Guid.Empty)
				throw new ArgumentException("PaymentId cannot be empty", nameof(request.PaymentId));
			if (request.InitiatedBy == Guid.Empty)
				throw new ArgumentException("InitiatedBy cannot be empty", nameof(request.InitiatedBy));
			ArgumentException.ThrowIfNullOrEmpty(request.Reason, nameof(request.Reason));
			ArgumentException.ThrowIfNullOrEmpty(request.Currency, nameof(request.Currency));

			if(request.RefundAmount <= 0)	
			{
				logger.LogWarning("Invalid refund amount: {RefundAmount} for Payment ID {PaymentId}", request.RefundAmount, request.PaymentId);
				throw new ArgumentOutOfRangeException(nameof(request.RefundAmount), "Refund amount must be greater than zero.");
			}

			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
			if (payment == null)
			{
				logger.LogWarning("Payment with ID {PaymentId} not found for refund.", request.PaymentId);
				throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");
			}

			try
			{
				var refund = payment.ProcessRefund(new Money(request.RefundAmount, request.Currency), request.Reason, request.InitiatedBy);
				logger.LogInformation("Initiating Paystack refund for Payment {PaymentId}, Refund {RefundId}", request.PaymentId, refund.Id);
				var paystackRefundId = await InitiatePaystackRefund(payment.ExternalTransactionId!,	request.RefundAmount, cancellationToken);
				refund.SetExternalRefundId(paystackRefundId);
				var refundEvent = payment.DomainEvents
				.OfType<RefundProcessedIntegrationEvent>()
				.FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(payment, cancellationToken);

					if (refundEvent != null)
					{
						await messageBroker.PublishAsync(refundEvent, cancellationToken);
					}

					payment.ClearEvents();
				}, cancellationToken);

				backgroundJob.Enqueue<IGmailService>(
				   "default",
				   emailService => emailService.SendEmailAsync(
					   request.RecipientEmail,
					   "REFUND INITIATED",
					   $"Your refund request of {request.RefundAmount} {request.Currency} for payment {request.PaymentId} has been submitted and is being processed. You will be notified once completed.",
					   false,
					   CancellationToken.None));

				logger.LogInformation("Refund initiated for Payment {PaymentId}, awaiting Paystack processing", request.PaymentId);
				return Unit.Value;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing refund for Payment ID {PaymentId}", request.PaymentId);
				throw;
			}
		}

		private async Task<string> InitiatePaystackRefund(string externalId, decimal refundAmount, CancellationToken cancellationToken)
		{
			var httpClient = httpClientFactory.CreateClient("PaystackClient");
			var requestBody = new
			{
				transaction = externalId,
				amount = (int)(refundAmount * 100)
			};
			var response = await httpClient.PostAsJsonAsync("/refund", requestBody, cancellationToken);
			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
				logger.LogError(new Exception(), "Paystack refund API failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
				throw new InvalidOperationException($"Paystack refund failed: {errorContent}");
			}
			var result = await response.Content.ReadFromJsonAsync<PaystackRefundResponse>(cancellationToken);

			if (result?.Status != true)
			{
				throw new InvalidOperationException($"Paystack refund failed: {result?.Message}");
			}

			logger.LogInformation("Paystack refund initiated: {RefundId}", result.Data.Id);
			return result.Data.Id.ToString();
		}

		private class PaystackRefundResponse
		{
			public bool Status { get; set; }
			public string? Message { get; set; }
			public RefundData Data { get; set; } = null!;
		}

		private class RefundData
		{
			public int Id { get; set; }
			public string Transaction { get; set; } = string.Empty;
			public string Status { get; set; } = string.Empty;
			public string Currency { get; set; } = string.Empty;
			public decimal Amount { get; set; }
		}
	}
}
