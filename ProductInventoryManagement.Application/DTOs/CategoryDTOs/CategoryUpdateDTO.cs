namespace ProductInventoryManagement.Application.DTOs.CategoryDTOs
{
	public record CategoryUpdateDTO
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
	}
}
