using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs;

public class NotificationPreferenceUpdateDTO
{
	// Channel Preferences
	public bool EmailEnabled { get; set; }
	public bool SmsEnabled { get; set; }
	public bool PushEnabled { get; set; }
	public bool InAppEnabled { get; set; }
	
	// Quiet Hours
	public TimeOnly? QuietHoursStart { get; set; }
	public TimeOnly? QuietHoursEnd { get; set; }
	
	public NotificationFrequency Frequency { get; set; }
}
