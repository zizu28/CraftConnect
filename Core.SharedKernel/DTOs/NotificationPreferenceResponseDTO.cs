using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationPreferenceResponseDTO
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public NotificationType NotificationType { get; set; }
	
	// Channel Preferences
	public bool EmailEnabled { get; set; }
	public bool SmsEnabled { get; set; }
	public bool PushEnabled { get; set; }
	public bool InAppEnabled { get; set; }
	
	// Quiet Hours
	public TimeOnly? QuietHoursStart { get; set; }
	public TimeOnly? QuietHoursEnd { get; set; }
	
	public NotificationFrequency Frequency { get; set; }
	
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
