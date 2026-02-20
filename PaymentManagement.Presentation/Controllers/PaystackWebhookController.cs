using Core.Logging;
using Core.SharedKernel.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.DTOs;
using PaymentManagement.Application.Services;
using System.Text;

namespace PaymentManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/webhooks/paystack")]
	public class PaystackWebhookController(
		IMediator mediator,
		IPaystackWebhookVerifier webhookVerifier,
		ILoggingService<PaystackWebhookController> logger) : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> ProcessWebhook([FromBody] PaystackWebhookPayload payload)
		{
			// Get the signature from the header
			var signature = Request.Headers["x-paystack-signature"].ToString();

			// Read the raw request body for signature verification
			Request.Body.Position = 0;
			using var reader = new StreamReader(Request.Body, Encoding.UTF8);
			var rawBody = await reader.ReadToEndAsync();

			// Verify the webhook signature
			if (!webhookVerifier.VerifySignature(signature, rawBody))
			{
				logger.LogWarning("Invalid Paystack webhook signature received for event: {Event}", payload.Event);
				return Unauthorized(new { error = "Invalid signature" });
			}
			logger.LogInformation("Received Paystack webhook event: {Event}, Data ID: {DataId}, Status: {Status}", payload.Event, payload.Data.Id, payload.Data.Status);

			try
			{
				// Route to appropriate handler based on event type
				switch (payload.Event)
				{
					case "refund.processed":
					case "refund.failed":
						await ProcessRefundWebhook(payload);
						break;

					case "charge.success":
						// TODO: Handle successful charge
						await ProcessChargeWebhook(payload);
						logger.LogInformation("Charge success webhook received but not yet handled");
						break;

					case "transfer.success":
					case "transfer.failed":
						// TODO: Handle transfer events
						await ProcessTransferWebhook(payload);
						logger.LogInformation("Transfer webhook received but not yet handled");
						break;

					default:
						logger.LogInformation("Unhandled webhook event type: {Event}", payload.Event);
						break;
				}

				return Ok(new { message = "Webhook processed successfully" });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing Paystack webhook for event: {Event}", payload.Event);

				return StatusCode(500, new { error = "Internal server error" });
			}
		}

		private async Task ProcessTransferWebhook(PaystackWebhookPayload payload)
		{
			throw new NotImplementedException();
		}

		private async Task ProcessChargeWebhook(PaystackWebhookPayload payload)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes refund-specific webhook events
		/// </summary>
		private async Task ProcessRefundWebhook(PaystackWebhookPayload payload)
		{
			var command = new ProcessPaystackWebhookCommand
			{
				RecipientEmail = GetRecipientEmail(payload),
				Status = MapPaystackStatusToRefundStatus(payload.Data.Status),
				Data = new Data
				{
					Id = payload.Data.Id,
					Status = payload.Data.Status
				}
			};

			await mediator.Send(command);
		}

		/// <summary>
		/// Extracts recipient email from the webhook payload
		/// </summary>
		private string GetRecipientEmail(PaystackWebhookPayload payload)
		{
			// Try to get email from various possible sources
			if (!string.IsNullOrWhiteSpace(payload.Data.RefundedBy))
				return payload.Data.RefundedBy;

			if (payload.Data.Transaction?.Customer?.Email != null)
				return payload.Data.Transaction.Customer.Email;

			// Fallback - this should be configured or handled differently in production
			logger.LogWarning("Could not extract recipient email from webhook payload, using default");
			return "support@example.com"; // TODO: Configure fallback email
		}

		/// <summary>
		/// Maps Paystack status strings to internal RefundStatus enum
		/// </summary>
		private static RefundStatus MapPaystackStatusToRefundStatus(string paystackStatus)
		{
			return paystackStatus.ToLower() switch
			{
				"success" => RefundStatus.Processed,
				"failed" => RefundStatus.Failed,
				"pending" => RefundStatus.Pending,
				"processing" => RefundStatus.Pending,
				_ => RefundStatus.Pending
			};
		}
	}
}
