using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class CancelInvoiceCommand : IRequest<Unit>
	{
		public Guid InvoiceId { get; set; }
		public string Reason { get; set; }
	}
}
