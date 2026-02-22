using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Commands.BookingCommands;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using MassTransit;
using MediatR;

namespace BookingManagement.Application.Consumers
{
	/// <summary>
	/// Consumes ConfirmBookingCommand from SAGA to confirm a booking after payment.
	/// Maps the MassTransit message to the MediatR Application command and delegates all
	/// domain logic to <see cref="BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers.ConfirmBookingCommandHandler"/>.
	/// </summary>
	public class ConfirmBookingCommandConsumer(
		IMessageBroker publishEndpoint,
		IMediator mediator,
		ILoggingService<ConfirmBookingCommandConsumer> logger) : IConsumer<ConfirmBookingCommand>
	{
		public async Task Consume(ConsumeContext<ConfirmBookingCommand> context)
		{
			var message = context.Message;
			logger.LogInformation("Confirming booking {BookingId} for SAGA {CorrelationId}",
				message.BookingId, message.CorrelationId);

			try
			{
				// Map the SharedKernel MassTransit message to the Application CQRS command.
				// The handler owns all domain logic (fetch → guard → confirm → publish event).
				var appCommand = new ConfirmBookingCommand
				{
					CorrelationId = message.CorrelationId,
					BookingId = message.BookingId,
					ConfirmedAt = DateTime.UtcNow
				};

				await mediator.Send(appCommand, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error confirming booking {BookingId}", message.BookingId);

				// Publish failure event so the SAGA can compensate
				await publishEndpoint.PublishAsync(new BookingCancelledIntegrationEvent(
					message.CorrelationId,
					message.BookingId,
					ex.Message), context.CancellationToken);
			}
		}
	}
}
