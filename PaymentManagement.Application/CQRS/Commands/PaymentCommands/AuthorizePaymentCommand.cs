using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class AuthorizePaymentCommand : IRequest
	{
		public Guid PaymentId { get; set; }
		public string RecipientEmail { get; set; }
		public string ExternalTransactionId { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? AuthorizationCode { get; set; }
		public decimal Amount { get; set; }
	}
}
