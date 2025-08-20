namespace ProductInventoryManagement.Application.DTOs.ProductDTOs
{
	public record ProductUpdateDTO
	{
		public Guid ProductId { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Price { get; set; }
		public Guid? CategoryId { get; set; }
		public Guid? CraftmanId { get; set; }
		public int? StockQuantity { get; set; }
		public bool? IsActive { get; set; }
	}
}