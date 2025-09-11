namespace PaymentManagement.Application.DTOs.InvoiceLineItemDTOs
{
	public class InvoiceLineItemCreateDTO
	{
		public required string Description { get; set; } = string.Empty;

		public required decimal UnitPrice { get; set; }

		public required int Quantity { get; set; }

		public string? ItemCode { get; set; }
	}
}
