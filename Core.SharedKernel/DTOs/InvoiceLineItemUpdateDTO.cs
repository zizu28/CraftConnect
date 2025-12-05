namespace Core.SharedKernel.DTOs
{
	public class InvoiceLineItemUpdateDTO
	{
		public required Guid LineItemId { get; set; }

		public string? Currency { get; set; }
		public string? Description { get; set; }

		public decimal? UnitPrice { get; set; }

		public int? Quantity { get; set; }

		public string? ItemCode { get; set; }
	}
}
