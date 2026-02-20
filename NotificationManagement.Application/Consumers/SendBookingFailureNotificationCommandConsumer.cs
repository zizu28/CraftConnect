using Core.Logging;
using Core.SharedKernel.Commands.NotificationCommands;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.EmailService.GmailService;
using MassTransit;

namespace NotificationManagement.Application.Consumers
{
	/// <summary>
	/// Consumes SendBookingFailureNotificationCommand from SAGA to send booking failure notification
	/// </summary>
	public class SendBookingFailureNotificationCommandConsumer(
		IBackgroundJobService backgroundJob,
		IPublishEndpoint publishEndpoint,
		ILoggingService<SendBookingFailureNotificationCommandConsumer> logger) : IConsumer<SendBookingFailureNotificationCommand>
	{
		public async Task Consume(ConsumeContext<SendBookingFailureNotificationCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Sending booking failure notification for booking {BookingId}, SAGA {CorrelationId}", 
				command.BookingId, command.CorrelationId);

			try
			{
				// Build email content
				var subject = "Booking Failed - CraftConnect";
				var body = $@"
					<h2>Booking Could Not Be Completed</h2>
					<p>Dear Customer,</p>
					<p>Unfortunately, we were unable to complete your booking.</p>
					{(!string.IsNullOrWhiteSpace(command.Reason) ? $"<p><strong>Reason:</strong> {command.Reason}</p>" : "")}
					<p>No charges have been made to your account. If you were charged, a refund will be processed shortly.</p>
					<p>Please try again or contact our support team if you need assistance.</p>
					<p>We apologize for the inconvenience.</p>
					<p>Best regards,<br/>CraftConnect Team</p>
				";

				backgroundJob.Enqueue<IGmailService>(
					"default", 
					email => email.SendEmailAsync(
						command.CustomerEmail,
						subject,
						body,
						true,
						CancellationToken.None));

				// Publish success event (SAGA may finalize)
				await publishEndpoint.Publish(new NotificationSentIntegrationEvent(
					Guid.NewGuid(),
					Guid.Empty,
					NotificationType.BookingCancelled,
					NotificationChannel.Email,
					DateTime.UtcNow), context.CancellationToken);

				logger.LogInformation("Booking failure email sent successfully to {Email}", command.CustomerEmail);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error sending booking failure email to {Email}", command.CustomerEmail);
				
				// Don't fail SAGA if notification fails
				// Just log - SAGA will complete anyway
			}
		}
	}
}
