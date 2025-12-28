using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationPreferenceCreateDTO
{
	public Guid UserId { get; set; }
	public NotificationType NotificationType { get; set; }
	
	// Channel Preferences
	public bool EmailEnabled { get; set; } = true;
	public bool SmsEnabled { get; set; } = false;
	public bool PushEnabled { get; set; } = false;
	public bool InAppEnabled { get; set; } = true;
	
	// Quiet Hours (optional)
	public TimeOnly? QuietHoursStart { get; set; }
	public TimeOnly? QuietHoursEnd { get; set; }
	
	public NotificationFrequency Frequency { get; set; } = NotificationFrequency.Immediate;
}
