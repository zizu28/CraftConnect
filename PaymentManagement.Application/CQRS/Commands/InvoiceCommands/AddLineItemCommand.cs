using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class AddLineItemCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public string Description { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public string? ItemCode { get; set; } 
	}
}
