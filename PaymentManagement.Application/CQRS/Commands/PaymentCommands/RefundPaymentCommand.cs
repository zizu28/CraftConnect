using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class RefundPaymentCommand : IRequest<bool>
	{
		public Guid PaymentId { get; set; }
		public string RecipientEmail { get; set; }
		public string Currency { get; set; }
		public decimal RefundAmount { get; set; }
		public string Reason { get; set; }
		public Guid InitiatedBy { get; set; }
	}
}
