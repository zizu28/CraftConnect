using System.ComponentModel.DataAnnotations;

namespace Core.SharedKernel.ValueObjects
{
	public record Project
	{
		public Guid ProjectId { get; private set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		[Timestamp]
		public byte[]? RowVersion { get; set; }

		public Project()
		{
			ProjectId = Guid.Empty;
			Title = string.Empty;
			Description = string.Empty;
			ImageUrl = string.Empty;
		}

		public Project(string title, string description, string imageUrl)
		{
			ProjectId = Guid.NewGuid();
			Title = title;
			Description = description;
			ImageUrl = imageUrl;
		}
	}
}
