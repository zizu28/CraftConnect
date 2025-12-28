using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationResponseDTO
{
	public Guid Id { get; set; }
	public Guid RecipientId { get; set; }
	public string RecipientEmail { get; set; } = string.Empty;
	
	public NotificationType Type { get; set; }
	public NotificationChannel Channel { get; set; }
	public NotificationPriority Priority { get; set; }
	public NotificationStatus Status { get; set; }
	
	public string Subject { get; set; } = string.Empty;
	public string Body { get; set; } = string.Empty;
	
	public int RetryCount { get; set; }
	public int MaxRetries { get; set; }
	
	public DateTime? ScheduledFor { get; set; }
	public DateTime? SentAt { get; set; }
	public DateTime? DeliveredAt { get; set; }
	public DateTime? FailedAt { get; set; }
	
	public string? ErrorMessage { get; set; }
	
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
