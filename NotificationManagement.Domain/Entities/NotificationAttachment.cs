using Core.SharedKernel.Domain;

namespace NotificationManagement.Domain.Entities
{
	public class NotificationAttachment : Entity
	{
		public Guid NotificationId { get; private set; }

		public string FileName { get; private set; }
		public string ContentType { get; private set; }
		public long FileSizeBytes { get; private set; }

		public byte[] Content { get; private set; } // For small files

		public bool IsInline { get; private set; }
		public string? ContentId { get; private set; }

		public DateTime CreatedAt { get; private set; }

		// EF Core constructor
		private NotificationAttachment() { }

		/// <summary>
		/// Internal factory - only Notification aggregate can create
		/// </summary>
		internal static NotificationAttachment Create(
			Guid notificationId,
			string fileName,
			string contentType,
			byte[] content,
			bool isInline = false,
			string? contentId = null)
		{
			if (content.Length > 5 * 1024 * 1024) // 5MB limit
				throw new InvalidOperationException("Attachment cannot exceed 5MB");

			return new NotificationAttachment
			{
				Id = Guid.NewGuid(),
				NotificationId = notificationId,
				FileName = fileName,
				ContentType = contentType,
				FileSizeBytes = content.Length,
				Content = content,
				IsInline = isInline,
				ContentId = contentId,
				CreatedAt = DateTime.UtcNow
			};
		}

		/// <summary>
		/// Mark as inline for HTML embedding
		/// </summary>
		internal void MarkAsInline(string contentId)
		{
			IsInline = true;
			ContentId = contentId;
		}
	}
}