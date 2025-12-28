using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationManagement.Domain.Entities
{
	public class NotificationPreference : AggregateRoot
	{
		public Guid UserId { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationType NotificationType { get; private set; }

		// Channel Preferences
		public bool EmailEnabled { get; private set; }
		public bool SmsEnabled { get; private set; }
		public bool PushEnabled { get; private set; }
		public bool InAppEnabled { get; private set; }

		// Quiet Hours
		public TimeOnly? QuietHoursStart { get; private set; }
		public TimeOnly? QuietHoursEnd { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationFrequency Frequency { get; private set; }

		public DateTime CreatedAt { get; private set; }
		public DateTime? UpdatedAt { get; private set; }

		[Timestamp]
		public byte[] RowVersion { get; private set; }

		private NotificationPreference() { }

		// BEHAVIORS

		public static NotificationPreference CreateDefault(Guid userId, NotificationType type)
		{
			return new NotificationPreference
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				NotificationType = type,
				EmailEnabled = true,  // Email on by default
				SmsEnabled = false,
				PushEnabled = false,
				InAppEnabled = true,
				Frequency = NotificationFrequency.Immediate,
				CreatedAt = DateTime.UtcNow
			};
		}

		public void UpdateChannelPreferences(bool email, bool sms, bool push, bool inApp)
		{
			EmailEnabled = email;
			SmsEnabled = sms;
			PushEnabled = push;
			InAppEnabled = inApp;
			UpdatedAt = DateTime.UtcNow;
		}

		public void SetQuietHours(TimeOnly start, TimeOnly end)
		{
			QuietHoursStart = start;
			QuietHoursEnd = end;
			UpdatedAt = DateTime.UtcNow;
		}

		public void ClearQuietHours()
		{
			QuietHoursStart = null;
			QuietHoursEnd = null;
			UpdatedAt = DateTime.UtcNow;
		}

		public bool IsChannelEnabled(NotificationChannel channel)
		{
			return channel switch
			{
				NotificationChannel.Email => EmailEnabled,
				NotificationChannel.SMS => SmsEnabled,
				NotificationChannel.Push => PushEnabled,
				NotificationChannel.InApp => InAppEnabled,
				_ => false
			};
		}

		public bool IsInQuietHours()
		{
			if (!QuietHoursStart.HasValue || !QuietHoursEnd.HasValue)
				return false;

			var now = TimeOnly.FromDateTime(DateTime.Now);

			// Handle overnight quiet hours (e.g., 22:00 - 08:00)
			if (QuietHoursStart > QuietHoursEnd)
				return now >= QuietHoursStart || now <= QuietHoursEnd;

			return now >= QuietHoursStart && now <= QuietHoursEnd;
		}
	}
}
