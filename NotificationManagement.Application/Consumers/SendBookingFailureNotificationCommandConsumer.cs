using Core.Logging;
using Core.SharedKernel.Commands.NotificationCommands;
using MassTransit;
using MediatR;

namespace NotificationManagement.Application.Consumers
{
	/// <summary>
	/// Consumes SendBookingFailureNotificationCommand from SAGA to send booking failure notification.
	/// Delegates all email-building and notification logic to
	/// <see cref="NotificationManagement.Application.CQRS.Handlers.CommandHandlers.NotificationCommandHandlers.SendBookingFailureNotificationCommandHandler"/>.
	/// </summary>
	public class SendBookingFailureNotificationCommandConsumer(
		IMediator mediator,
		ILoggingService<SendBookingFailureNotificationCommandConsumer> logger)
		: IConsumer<SendBookingFailureNotificationCommand>
	{
		public async Task Consume(ConsumeContext<SendBookingFailureNotificationCommand> context)
		{
			var message = context.Message;
			logger.LogInformation(
				"Sending booking failure notification for booking {BookingId}, SAGA {CorrelationId}",
				message.BookingId, message.CorrelationId);

			try
			{
				var appCommand = new CQRS.Commands.NotificationCommands.SendBookingFailureNotificationCommand
				{
					CorrelationId = message.CorrelationId,
					BookingId = message.BookingId,
					RecipientId = message.RecipientId,
					CustomerEmail = message.CustomerEmail,
					Reason = message.Reason
				};

				await mediator.Send(appCommand, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error sending booking failure email to {Email}", message.CustomerEmail);
				// Notification failure is non-critical — SAGA continues regardless. Just log.
			}
		}
	}
}
