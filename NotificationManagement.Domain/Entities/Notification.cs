using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace NotificationManagement.Domain.Entities
{
	public class Notification : AggregateRoot
	{
		public Guid UserId { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationChannel NotificationChannel { get; private set; }
		public string Content { get; private set; } = string.Empty;
		public DateTime Timestamp { get; private set; }
		public DateTime? ReadTimestamp { get; private set; }
		public bool IsRead { get; private set; }
		public NotificationContext NotificationContext { get; private set; } = new("", Guid.Empty);

		private Notification() { }

		public static Notification Create(Guid userId, string channel, string content, NotificationContext context)
		{
			if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
			ArgumentException.ThrowIfNullOrEmpty(channel);
			ArgumentException.ThrowIfNullOrEmpty(content);
			ArgumentNullException.ThrowIfNull(context);
			return new Notification
			{
				UserId = userId,
				NotificationChannel = Enum.Parse<NotificationChannel>(channel, true),
				Content = content,
				NotificationContext = context
			};
		}

		public void MarkAsRead()
		{
			IsRead = true;
			ReadTimestamp = DateTime.UtcNow;
		}
	}
}
