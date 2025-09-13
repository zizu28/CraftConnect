using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class RemoveLineItemCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public Guid LineItemId { get; set; }
	}
}
