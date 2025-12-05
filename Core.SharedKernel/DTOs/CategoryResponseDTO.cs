namespace Core.SharedKernel.DTOs
{
	public record CategoryResponseDTO
	{
		public Guid CategoryId { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public Guid? ParentCategoryId { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime LastModified { get; set; }
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = string.Empty;
		public List<string> Errors { get; set; } = [];
	}
}
