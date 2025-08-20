namespace Core.SharedKernel.ValueObjects
{
	public record Image
	{
		public string Url { get; private set; }
		public string AltText { get; private set; }
		private Image()
		{
			Url = string.Empty;
			AltText = string.Empty;
		}
		public Image(string url, string altText)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentException("Image URL cannot be empty.", nameof(url));
			if (string.IsNullOrWhiteSpace(altText))
				throw new ArgumentException("Alt text cannot be empty.", nameof(altText));
			Url = url;
			AltText = altText;
		}
	}
}
