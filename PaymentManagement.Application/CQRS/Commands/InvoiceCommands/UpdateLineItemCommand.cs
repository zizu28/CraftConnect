using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class UpdateLineItemCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public Guid LineItemId { get; set; }
		public string? Description { get; set; }
		public decimal? UnitPrice { get; set; }
		public int? Quantity { get; set; }
	}
}
