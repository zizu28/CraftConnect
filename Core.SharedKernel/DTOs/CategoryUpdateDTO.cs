namespace Core.SharedKernel.DTOs
{
	public record CategoryUpdateDTO
	{
		public Guid CategoryId { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
	}
}
