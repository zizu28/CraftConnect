namespace ProductInventoryManagement.Application.DTOs.ProductDTOs
{
	public record ProductCreateDTO
	{
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required decimal Price { get; set; }
		public required Guid CategoryId { get; set; }
		public required Guid CraftmanId { get; set; }
		public required int StockQuantity { get; set; }
	}
}