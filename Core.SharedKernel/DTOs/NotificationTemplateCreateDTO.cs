using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationTemplateCreateDTO
{
	public string Name { get; set; } = string.Empty;
	public string Code { get; set; } = string.Empty;
	public NotificationType Type { get; set; }
	public NotificationChannel Channel { get; set; }
	
	public string SubjectTemplate { get; set; } = string.Empty;
	public string BodyTemplate { get; set; } = string.Empty;
	public string? Description { get; set; }
	
	public List<string> RequiredVariables { get; set; } = new();
	public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Normal;
	public int DefaultMaxRetries { get; set; } = 3;
}
