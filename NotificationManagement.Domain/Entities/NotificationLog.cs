using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using System.Text.Json.Serialization;

namespace NotificationManagement.Domain.Entities
{
	public class NotificationLog : Entity
	{
		public Guid NotificationId { get; private set; }

		public int AttemptNumber { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationStatus Status { get; private set; }
		public DateTime AttemptedAt { get; private set; }

		public string Message { get; private set; }
		public string? Provider { get; private set; }
		public string? ExternalId { get; private set; }
		public string? ErrorCode { get; private set; }

		// EF Core constructor
		private NotificationLog() { }

		/// <summary>
		/// Internal factory - only Notification aggregate can call this
		/// </summary>
		internal static NotificationLog Create(
			Guid notificationId,
			int attemptNumber,
			NotificationStatus status,
			string message,
			string? provider = null,
			string? externalId = null,
			string? errorCode = null)
		{
			return new NotificationLog
			{
				Id = Guid.NewGuid(),
				NotificationId = notificationId,
				AttemptNumber = attemptNumber,
				Status = status,
				Message = message,
				Provider = provider,
				ExternalId = externalId,
				ErrorCode = errorCode,
				AttemptedAt = DateTime.UtcNow
			};
		}
	}
}