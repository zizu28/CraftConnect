using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using MediatR;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;

namespace NotificationManagement.Application.CQRS.Handlers.CommandHandlers.NotificationCommandHandlers
{
	public class SendBookingFailureNotificationCommandHandler(
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker,
		ILoggingService<SendBookingFailureNotificationCommandHandler> logger)
		: IRequestHandler<SendBookingFailureNotificationCommand, Unit>
	{
		public async Task<Unit> Handle(
			SendBookingFailureNotificationCommand request,
			CancellationToken cancellationToken)
		{
			var subject = "Booking Failed - CraftConnect";
			var body = $@"
				<h2>Booking Could Not Be Completed</h2>
				<p>Dear Customer,</p>
				<p>Unfortunately, we were unable to complete your booking.</p>
				{(!string.IsNullOrWhiteSpace(request.Reason)
					? $"<p><strong>Reason:</strong> {request.Reason}</p>"
					: "")}
				<p>No charges have been made to your account. If you were charged, a refund will be processed shortly.</p>
				<p>Please try again or contact our support team if you need assistance.</p>
				<p>We apologize for the inconvenience.</p>
				<p>Best regards,<br/>CraftConnect Team</p>
			";

			backgroundJob.Enqueue<IGmailService>(
				"default",
				email => email.SendEmailAsync(
					request.CustomerEmail,
					subject,
					body,
					true,
					CancellationToken.None));

			await messageBroker.PublishAsync(new NotificationSentIntegrationEvent(
				request.CorrelationId,
				request.BookingId,
				request.RecipientId,
				NotificationType.BookingCancelled,
				NotificationChannel.Email,
				DateTime.UtcNow), cancellationToken);

			logger.LogInformation(
				"Booking failure email enqueued for {Email}, SAGA {CorrelationId}",
				request.CustomerEmail, request.CorrelationId);

			return Unit.Value;
		}
	}
}
