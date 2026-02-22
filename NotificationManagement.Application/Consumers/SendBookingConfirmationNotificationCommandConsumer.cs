using Core.Logging;
using Core.SharedKernel.Commands.NotificationCommands;
using Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents;
using Core.EventServices;
using MassTransit;
using MediatR;
using Core.SharedKernel.Enums;

namespace NotificationManagement.Application.Consumers
{
	/// <summary>
	/// Consumes SendBookingConfirmationNotificationCommand from SAGA to send booking confirmation email.
	/// Delegates all email-building and notification logic to
	/// <see cref="NotificationManagement.Application.CQRS.Handlers.CommandHandlers.NotificationCommandHandlers.SendBookingConfirmationNotificationCommandHandler"/>.
	/// </summary>
	public class SendBookingConfirmationNotificationCommandConsumer(
		IMediator mediator,
		IMessageBroker publishEndpoint,
		ILoggingService<SendBookingConfirmationNotificationCommandConsumer> logger)
		: IConsumer<SendBookingConfirmationNotificationCommand>
	{
		public async Task Consume(ConsumeContext<SendBookingConfirmationNotificationCommand> context)
		{
			var message = context.Message;
			logger.LogInformation(
				"Sending booking confirmation notification for booking {BookingId}, SAGA {CorrelationId}",
				message.BookingId, message.CorrelationId);

			try
			{
				var appCommand = new SendBookingConfirmationNotificationCommand
				{
					CorrelationId = message.CorrelationId,
					BookingId = message.BookingId,
					RecipientId = message.RecipientId,
					CustomerEmail = message.CustomerEmail,
					ServiceDescription = message.ServiceDescription,
					ScheduledDate = message.ScheduledDate,
					Amount = message.Amount,
					Currency = message.Currency,
					PaymentReference = message.PaymentReference
				};

				await mediator.Send(appCommand, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error sending booking confirmation email to {Email}", message.CustomerEmail);

				await publishEndpoint.PublishAsync(new NotificationFailedIntegrationEvent(
					message.CorrelationId,
					message.BookingId,
					message.RecipientId,
					NotificationType.BookingConfirmed,
					$"Failed to send email: {ex.Message}",
					DateTime.UtcNow), context.CancellationToken);
			}
		}
	}
}
