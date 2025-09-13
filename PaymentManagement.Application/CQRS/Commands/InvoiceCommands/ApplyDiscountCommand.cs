using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.InvoiceCommands
{
	public class ApplyDiscountCommand : IRequest
	{
		public Guid InvoiceId { get; set; }
		public decimal DiscountAmount { get; set; }
		public string Currency { get; set; } = "USD";
	}
}
