using Core.Logging;
using Core.SharedKernel.Enums;
using Infrastructure.EmailService;
using Infrastructure.EmailService.GmailService;
using NotificationManagement.Application.Contracts;

namespace NotificationManagement.Infrastructure.Providers;

public class EmailNotificationProvider(
	IGmailService emailService,
	ILoggingService<EmailNotificationProvider> logger) : INotificationProvider
{
	public async Task<(bool success, string? externalId, string? errorMessage)> SendAsync(
		NotificationChannel channel,
		string recipient,
		string subject,
		string body,
		byte[]? attachmentContent = null,
		string? attachmentFileName = null,
		CancellationToken cancellationToken = default)
	{
		if (channel != NotificationChannel.Email)
		{
			return (false, null, $"Channel {channel} not supported by EmailNotificationProvider");
		}

		try
		{
			// Use existing email service
			await emailService.SendEmailAsync(recipient, subject, body, cancellationToken: cancellationToken);
			
			logger.LogInformation("Email sent successfully to {Recipient}", recipient);
			
			// Return success with no external ID (gmail service doesn't provide one)
			return (true, null, null);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to send email to {Recipient}", recipient);
			return (false, null, ex.Message);
		}
	}

	public bool SupportsChannel(NotificationChannel channel)
	{
		return channel == NotificationChannel.Email;
	}
}
