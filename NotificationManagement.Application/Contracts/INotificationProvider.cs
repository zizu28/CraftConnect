using Core.SharedKernel.Enums;

namespace NotificationManagement.Application.Contracts;

/// <summary>
/// Service for sending notifications through various channels
/// </summary>
public interface INotificationProvider
{
	Task<(bool success, string? externalId, string? errorMessage)> SendAsync(
		NotificationChannel channel,
		string recipient,
		string subject,
		string body,
		byte[]? attachmentContent = null,
		string? attachmentFileName = null,
		CancellationToken cancellationToken = default);
		
	bool SupportsChannel(NotificationChannel channel);
}
