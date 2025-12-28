using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationCreateDTO
{
	public Guid RecipientId { get; set; }
	public string RecipientEmail { get; set; } = string.Empty;
	public string? RecipientPhone { get; set; }
	
	public NotificationType Type { get; set; }
	public NotificationChannel Channel { get; set; }
	public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
	
	public string Subject { get; set; } = string.Empty;
	public string Body { get; set; } = string.Empty;
	
	public Dictionary<string, string>? Metadata { get; set; }
	public DateTime? ScheduledFor { get; set; }
	
	// Template-based creation (optional)
	public Guid? TemplateId { get; set; }
	public Dictionary<string, string>? TemplateVariables { get; set; }
}
