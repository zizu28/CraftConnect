namespace ProductInventoryManagement.Application.DTOs.ProductDTOs
{
	public record ProductResponseDTO
	{
		public Guid ProductId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public Guid CategoryId { get; set; }
		public Guid CraftmanId { get; set; }
		public bool IsActive { get; set; }
		public List<ImageResponseDTO> Images { get; set; } = [];
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = string.Empty;
		public List<string> Errors { get; set; } = [];
	}
}