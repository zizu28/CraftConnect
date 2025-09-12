using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class DeleteInvoiceCommand : IRequest<Unit>
	{
		public Guid InvoiceId { get; set; }
		public string Reason { get; set; } = string.Empty;
	}
}
