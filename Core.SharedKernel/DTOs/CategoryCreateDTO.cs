namespace Core.SharedKernel.DTOs
{
	public record CategoryCreateDTO
	{
		public required string Name { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
	}
}
