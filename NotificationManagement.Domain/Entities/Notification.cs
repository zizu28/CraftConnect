using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotificationManagement.Domain.Entities
{
	public class Notification : AggregateRoot
	{
		// Recipient Information
		public Guid RecipientId { get; private set; }
		public string RecipientEmail { get; private set; }
		public string? RecipientPhone { get; private set; }

		// Notification Details
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationType Type { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationChannel Channel { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationPriority Priority { get; private set; }

		// Content
		public string Subject { get; private set; }
		public string Body { get; private set; }
		[NotMapped]
		public Dictionary<string, string>? Metadata { get; private set; }

		// Delivery Status
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationStatus Status { get; private set; }
		public int RetryCount { get; private set; }
		public int MaxRetries { get; private set; }
		public DateTime? ScheduledFor { get; private set; }
		public DateTime? SentAt { get; private set; }
		public DateTime? DeliveredAt { get; private set; }
		public DateTime? FailedAt { get; private set; }

		// Error Tracking
		public string? ErrorMessage { get; private set; }

		// Template Reference
		public Guid? TemplateId { get; private set; }

		// CHILD ENTITIES (Owned Collections)
		private readonly List<NotificationLog> _logs = [];
		public IReadOnlyCollection<NotificationLog> Logs => _logs.AsReadOnly();

		private readonly List<NotificationAttachment> _attachments = [];
		public IReadOnlyCollection<NotificationAttachment> Attachments => _attachments.AsReadOnly();

		// Audit
		public DateTime CreatedAt { get; private set; }
		public DateTime? UpdatedAt { get; private set; }

		[Timestamp]
		public byte[] RowVersion { get; private set; }

		// EF Core constructor
		private Notification() { }

		//===============================================
		// AGGREGATE ROOT BEHAVIORS
		//===============================================

		/// <summary>
		/// Factory method to create new notification
		/// </summary>
		public static Notification Create(
			Guid recipientId,
			string recipientEmail,
			NotificationType type,
			NotificationChannel channel,
			string subject,
			string body,
			NotificationPriority priority = NotificationPriority.Normal,
			Guid? templateId = null,
			Dictionary<string, string>? metadata = null)
		{
			var notification = new Notification
			{
				Id = Guid.NewGuid(),
				RecipientId = recipientId,
				RecipientEmail = recipientEmail,
				Type = type,
				Channel = channel,
				Subject = subject,
				Body = body,
				Priority = priority,
				Status = NotificationStatus.Pending,
				RetryCount = 0,
				MaxRetries = 3,
				TemplateId = templateId,
				Metadata = metadata,
				CreatedAt = DateTime.UtcNow
			};

			// Log creation
			notification.AddLog(NotificationStatus.Pending, "Notification created");

			return notification;
		}

		/// <summary>
		/// Schedule for future delivery
		/// </summary>
		public void Schedule(DateTime scheduledFor)
		{
			if (scheduledFor <= DateTime.UtcNow)
				throw new InvalidOperationException("Scheduled time must be in the future");

			ScheduledFor = scheduledFor;
			Status = NotificationStatus.Scheduled;
			UpdatedAt = DateTime.UtcNow;

			AddLog(NotificationStatus.Scheduled, $"Scheduled for {scheduledFor:u}");
		}

		/// <summary>
		/// Add attachment to notification (only for Email channel)
		/// </summary>
		public void AddAttachment(string fileName, string contentType, byte[] content)
		{
			if (Channel != NotificationChannel.Email)
				throw new InvalidOperationException("Attachments only supported for email notifications");

			if (Status != NotificationStatus.Pending && Status != NotificationStatus.Scheduled)
				throw new InvalidOperationException("Cannot add attachments to notification that is already being sent");

			var attachment = NotificationAttachment.Create(Id, fileName, contentType, content);
			_attachments.Add(attachment);
			UpdatedAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Remove attachment
		/// </summary>
		public void RemoveAttachment(Guid attachmentId)
		{
			if (Status != NotificationStatus.Pending && Status != NotificationStatus.Scheduled)
				throw new InvalidOperationException("Cannot remove attachments from notification that is being sent");

			var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
			if (attachment != null)
			{
				_attachments.Remove(attachment);
				UpdatedAt = DateTime.UtcNow;
			}
		}

		/// <summary>
		/// Mark as sending
		/// </summary>
		public void MarkAsSending()
		{
			if (!CanSendNow())
				throw new InvalidOperationException("Notification cannot be sent in current state");

			Status = NotificationStatus.Sending;
			UpdatedAt = DateTime.UtcNow;

			AddLog(NotificationStatus.Sending, "Sending notification");
		}

		/// <summary>
		/// Mark as successfully sent
		/// </summary>
		public void MarkAsSent(string? provider = null, string? externalId = null)
		{
			Status = NotificationStatus.Sent;
			SentAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;

			AddLog(NotificationStatus.Sent, "Notification sent successfully", provider, externalId);

			// Raise domain event
			AddIntegrationEvent(new NotificationSentIntegrationEvent(
				Id, RecipientId, Type, Channel, SentAt.Value));
		}

		/// <summary>
		/// Mark as delivered (confirmed by provider)
		/// </summary>
		public void MarkAsDelivered()
		{
			if (Status != NotificationStatus.Sent)
				throw new InvalidOperationException("Cannot mark as delivered if not sent");

			Status = NotificationStatus.Delivered;
			DeliveredAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;

			AddLog(NotificationStatus.Delivered, "Notification delivered");
		}

		/// <summary>
		/// Mark as failed - handles retry logic
		/// </summary>
		public bool MarkAsFailed(string errorMessage, string? provider = null, string? errorCode = null)
		{
			RetryCount++;
			ErrorMessage = errorMessage;
			UpdatedAt = DateTime.UtcNow;

			AddLog(NotificationStatus.Failed, errorMessage, provider, null, errorCode);

			if (RetryCount >= MaxRetries)
			{
				Status = NotificationStatus.Failed;
				FailedAt = DateTime.UtcNow;

				// Raise failure event
				AddIntegrationEvent(new NotificationFailedIntegrationEvent(
					Id, RecipientId, Type, errorMessage, FailedAt.Value));

				return false; // No more retries
			}

			Status = NotificationStatus.Pending; // Reset for retry
			return true; // Can retry
		}

		/// <summary>
		/// Cancel notification
		/// </summary>
		public void Cancel(string reason)
		{
			if (Status == NotificationStatus.Sent || Status == NotificationStatus.Delivered)
				throw new InvalidOperationException("Cannot cancel already sent notification");

			Status = NotificationStatus.Cancelled;
			UpdatedAt = DateTime.UtcNow;

			AddLog(NotificationStatus.Cancelled, reason);
		}

		/// <summary>
		/// Check if can send now
		/// </summary>
		public bool CanSendNow()
		{
			if (Status == NotificationStatus.Cancelled || Status == NotificationStatus.Failed)
				return false;

			if (Status == NotificationStatus.Sent || Status == NotificationStatus.Delivered)
				return false;

			if (ScheduledFor.HasValue && ScheduledFor.Value > DateTime.UtcNow)
				return false;

			return true;
		}

		//===============================================
		// PRIVATE HELPER - Manages Child Entity
		//===============================================

		/// <summary>
		/// Add log entry (private - only aggregate root can add logs)
		/// </summary>
		private void AddLog(
			NotificationStatus status,
			string message,
			string? provider = null,
			string? externalId = null,
			string? errorCode = null)
		{
			var log = NotificationLog.Create(
				Id,
				RetryCount,
				status,
				message,
				provider,
				externalId,
				errorCode);

			_logs.Add(log);
		}
	}
}
