using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using System.Security.Cryptography;
using System.Text;

namespace PaymentManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/webhooks/paystack")]
	public class PaystackWebhookController(
		IMediator mediator,
		IConfiguration configuration) : ControllerBase
	{
		[HttpPost("refund")]
		public async Task<IActionResult> ProcessRefundWebhook([FromBody] PaystackWebhookPayload payload)
		{
			if (!VerifyPaystackSignature(Request.Headers["x-paystack-signature"], payload))
			{
				return Unauthorized();
			}
			var command = new ProcessPaystackWebhookCommand
			{
				RecipientEmail = payload.Data.RecipientEmail, // You'll need to get this
				Status = MapStatus(payload.Data.Status),
				Data = new Data
				{
					Id = payload.Data.Id,
					Status = payload.Data.Status
				}
			};

			await mediator.Send(command);

			return Ok();
		}

		private bool VerifyPaystackSignature(string signature, string payload)
		{
			using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(configuration["Paystack:SecretKey"]!));
			var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
			var computedSignature = Convert.ToHexStringLower(hash);

			return signature == computedSignature;
		}
	}
}
