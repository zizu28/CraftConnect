using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class MarkAsPaidCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public Guid PaymentId { get; set; }
		public decimal AmountPaid { get; set; }
		public string Currency { get; set; } = "USD";
		public string RecipientEmail { get; set; }
	}
}
