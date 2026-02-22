using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using MediatR;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;
using NotificationManagement.Application.Validators;

namespace NotificationManagement.Application.CQRS.Handlers.CommandHandlers.NotificationCommandHandlers
{
	public class SendBookingConfirmationNotificationCommandHandler(
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker,
		ILoggingService<SendBookingConfirmationNotificationCommandHandler> logger)
		: IRequestHandler<SendBookingConfirmationNotificationCommand, Unit>
	{
		public async Task<Unit> Handle(
			SendBookingConfirmationNotificationCommand request,
			CancellationToken cancellationToken)
		{
			var validator = new SendBookingConfirmationNotificationCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			var subject = "Booking Confirmation - CraftConnect";
			var body = $@"
				<h2>Booking Confirmed!</h2>
				<p>Dear Customer,</p>
				<p>Your booking has been confirmed successfully.</p>
				<h3>Booking Details:</h3>
				<ul>
					<li><strong>Service:</strong> {request.ServiceDescription}</li>
					<li><strong>Amount Paid:</strong> {request.Currency} {request.Amount:N2}</li>
					<li><strong>Payment Reference:</strong> {request.PaymentReference ?? "N/A"}</li>
					{(request.ScheduledDate.HasValue
						? $"<li><strong>Scheduled Date:</strong> {request.ScheduledDate.Value:MMMM dd, yyyy}</li>"
						: "")}
				</ul>
				<p>Thank you for choosing CraftConnect!</p>
			";

			backgroundJob.Enqueue<IGmailService>(
				"default",
				gmail => gmail.SendEmailAsync(
					request.CustomerEmail,
					subject,
					body,
					true,
					CancellationToken.None));

			await messageBroker.PublishAsync(new NotificationSentIntegrationEvent(
				request.CorrelationId,
				request.BookingId,
				request.RecipientId,
				NotificationType.BookingConfirmed,
				NotificationChannel.Email,
				DateTime.UtcNow), cancellationToken);

			logger.LogInformation(
				"Booking confirmation email enqueued for {Email}, SAGA {CorrelationId}",
				request.CustomerEmail, request.CorrelationId);

			return Unit.Value;
		}
	}
}
