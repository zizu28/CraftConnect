using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationTemplateResponseDTO
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Code { get; set; } = string.Empty;
	public NotificationType Type { get; set; }
	public NotificationChannel Channel { get; set; }
	
	public string SubjectTemplate { get; set; } = string.Empty;
	public string BodyTemplate { get; set; } = string.Empty;
	public string? Description { get; set; }
	
	public bool IsActive { get; set; }
	public NotificationPriority DefaultPriority { get; set; }
	public int DefaultMaxRetries { get; set; }
	
	public List<string> RequiredVariables { get; set; } = [];
	public int Version { get; set; }
	
	public DateTime CreatedAt { get; set; }
	public Guid CreatedBy { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
