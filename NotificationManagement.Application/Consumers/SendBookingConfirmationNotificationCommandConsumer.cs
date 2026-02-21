using Core.EventServices;
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
	/// Consumes SendBookingConfirmationNotificationCommand from SAGA to send booking confirmation email
	/// </summary>
	public class SendBookingConfirmationNotificationCommandConsumer(
		IBackgroundJobService backgroundJob,
		IMessageBroker publishEndpoint,
		ILoggingService<SendBookingConfirmationNotificationCommandConsumer> logger) : IConsumer<SendBookingConfirmationNotificationCommand>
	{
		public async Task Consume(ConsumeContext<SendBookingConfirmationNotificationCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Sending booking confirmation notification for booking with ID {BookingId}, SAGA {CorrelationId}", 
				command.BookingId, command.CorrelationId);

			try
			{
				// Build email content
				var subject = "Booking Confirmation - CraftConnect";
				var body = $@"
					<h2>Booking Confirmed!</h2>
					<p>Dear Customer,</p>
					<p>Your booking has been confirmed successfully.</p>
					<h3>Booking Details:</h3>
					<ul>
						<li><strong>Service:</strong> {command.ServiceDescription}</li>
						<li><strong>Amount Paid:</strong> {command.Currency} {command.Amount:N2}</li>
						<li><strong>Payment Reference:</strong> {command.PaymentReference ?? "N/A"}</li>
						{(command.ScheduledDate.HasValue ? $"<li><strong>Scheduled Date:</strong> {command.ScheduledDate.Value:MMMM dd, yyyy}</li>" : "")}
					</ul>
					<p>Thank you for choosing CraftConnect!</p>
				";

				backgroundJob.Enqueue<IGmailService>(
					"default", gmail => gmail.SendEmailAsync(
						command.CustomerEmail,
						subject,
						body,
						true,
						CancellationToken.None));

				// Publish success event back to SAGA
				await publishEndpoint.PublishAsync(new NotificationSentIntegrationEvent(
					Guid.NewGuid(),
					Guid.NewGuid(),
					command.RecipientId, // Recipient ID (would need to be passed in command)
					NotificationType.BookingConfirmed,
					NotificationChannel.Email,
					DateTime.UtcNow), context.CancellationToken);

				logger.LogInformation("Booking confirmation email sent successfully to {Email}", command.CustomerEmail);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error sending booking confirmation email to {Email}", command.CustomerEmail);
				
				// Publish failure event (optional - SAGA may complete anyway)
				await publishEndpoint.PublishAsync(new NotificationFailedIntegrationEvent(
					Guid.NewGuid(),
					Guid.Empty,
					command.RecipientId,
					NotificationType.BookingConfirmed,
					$"Failed to send email: {ex.Message}",
					DateTime.UtcNow), context.CancellationToken);
			}
		}
	}
}
