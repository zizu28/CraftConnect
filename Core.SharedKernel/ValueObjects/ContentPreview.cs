namespace Core.SharedKernel.ValueObjects
{
	public record ContentPreview
	{
		public string Title { get; private set; } = string.Empty;
		public string Description { get; private set; } = string.Empty;
		public string ImageUrl { get; private set; } = string.Empty;

		private ContentPreview()
		{
			Title = string.Empty;
			Description = string.Empty;
			ImageUrl = string.Empty;
		}

		public ContentPreview(string title, string description, string imageUrl)
		{
			ArgumentNullException.ThrowIfNullOrEmpty(title);
			ArgumentNullException.ThrowIfNullOrEmpty(description);
			ArgumentNullException.ThrowIfNullOrEmpty(imageUrl);
			Title = title;
			Description = description;
			ImageUrl = imageUrl;
		}
	}
}
