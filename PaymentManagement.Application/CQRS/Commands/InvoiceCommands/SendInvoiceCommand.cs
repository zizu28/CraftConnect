using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class SendInvoiceCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public string RecipientEmail { get; set; }
	}
}
