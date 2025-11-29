namespace Core.SharedKernel.ValueObjects
{
	public record Project
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }

		private Project()
		{
			Title = string.Empty;
			Description = string.Empty;
			ImageUrl = string.Empty;
		}

		public Project(string title, string description, string imageUrl)
		{
			ArgumentException.ThrowIfNullOrEmpty(title);
			ArgumentException.ThrowIfNullOrEmpty(description);
			ArgumentException.ThrowIfNullOrEmpty(imageUrl);
			Title = title;
			Description = description;
			ImageUrl = imageUrl;
		}
	}
}
