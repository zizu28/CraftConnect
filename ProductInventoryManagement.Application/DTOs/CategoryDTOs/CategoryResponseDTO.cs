namespace ProductInventoryManagement.Application.DTOs.CategoryDTOs
{
	public record CategoryResponseDTO
	{
		public Guid CategoryId { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime LastModified { get; set; }
	}
}
