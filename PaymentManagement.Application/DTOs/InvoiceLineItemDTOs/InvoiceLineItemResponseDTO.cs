namespace PaymentManagement.Application.DTOs.InvoiceLineItemDTOs
{
	public class InvoiceLineItemResponseDTO
	{
		public Guid Id { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal TotalPrice { get; set; }
		public string Currency { get; set; } = string.Empty;
		public string? ItemCode { get; set; }
	}
}
