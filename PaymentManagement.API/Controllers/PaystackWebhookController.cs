using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.DTOs;
using PaymentManagement.Application.Services;
using System.Text;

namespace PaymentManagement.API.Controllers
{
	/// <summary>
	/// Controller for receiving and processing Paystack webhook notifications
	/// </summary>
	[ApiController]
	[Route("api/webhooks/paystack")]
	public class PaystackWebhookController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IPaystackWebhookVerifier _webhookVerifier;
		private readonly ILogger<PaystackWebhookController> _logger;

		public PaystackWebhookController(
			IMediator mediator,
			IPaystackWebhookVerifier webhookVerifier,
			ILogger<PaystackWebhookController> logger)
		{
			_mediator = mediator;
			_webhookVerifier = webhookVerifier;
			_logger = logger;
		}

		/// <summary>
		/// Receives webhook notifications from Paystack
		/// </summary>
		/// <param name="payload">The webhook payload from Paystack</param>
		/// <returns>200 OK if processed successfully, 401 if signature invalid</returns>
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
			if (!_webhookVerifier.VerifySignature(signature, rawBody))
			{
				_logger.LogWarning("Invalid Paystack webhook signature received for event: {Event}", payload.Event);
				return Unauthorized(new { error = "Invalid signature" });
			}

			_logger.LogInformation("Received Paystack webhook event: {Event}, Data ID: {DataId}, Status: {Status}",
				payload.Event, payload.Data.Id, payload.Data.Status);

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
						_logger.LogInformation("Charge success webhook received but not yet handled");
						break;

					case "transfer.success":
					case "transfer.failed":
						// TODO: Handle transfer events
						_logger.LogInformation("Transfer webhook received but not yet handled");
						break;

					default:
						_logger.LogInformation("Unhandled webhook event type: {Event}", payload.Event);
						break;
				}

				return Ok(new { message = "Webhook processed successfully" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing Paystack webhook for event: {Event}", payload.Event);
				
				// Return 500 so Paystack will retry
				return StatusCode(500, new { error = "Internal server error" });
			}
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

			await _mediator.Send(command);
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
			_logger.LogWarning("Could not extract recipient email from webhook payload, using default");
			return "support@example.com"; // TODO: Configure fallback email
		}

		/// <summary>
		/// Maps Paystack status strings to internal RefundStatus enum
		/// </summary>
		private Core.SharedKernel.Enums.RefundStatus MapPaystackStatusToRefundStatus(string paystackStatus)
		{
			return paystackStatus.ToLower() switch
			{
				"success" => Core.SharedKernel.Enums.RefundStatus.Processed,
				"failed" => Core.SharedKernel.Enums.RefundStatus.Failed,
				"pending" => Core.SharedKernel.Enums.RefundStatus.Pending,
				"processing" => Core.SharedKernel.Enums.RefundStatus.Pending,
				_ => Core.SharedKernel.Enums.RefundStatus.Pending
			};
		}
	}
}
