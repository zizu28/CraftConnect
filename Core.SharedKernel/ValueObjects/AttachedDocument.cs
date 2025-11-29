namespace Core.SharedKernel.ValueObjects
{
	public record AttachedDocument
	{
		public string FileName { get; private set; }
		public string FileSize { get; private set; }
		public string DownloadUrl { get; private set; }

		private AttachedDocument()
		{
			FileName = string.Empty;
			FileSize = string.Empty;
			DownloadUrl = string.Empty;
		}

		public AttachedDocument(string fileName, string fileSize, string downloadUrl)
		{
			if (string.IsNullOrEmpty(fileName)) throw new("Filename cannot be null");
			if (string.IsNullOrEmpty(fileSize)) throw new("Filesize cannot be zero or null");
			if (string.IsNullOrEmpty(downloadUrl)) throw new("Download url cannot be null");
			FileName = fileName;
			FileSize = fileSize;
			DownloadUrl = downloadUrl;
		}
	}
}
