using Core.Logging;
using Core.SharedKernel.Commands.BookingCommands;
using MassTransit;
using MediatR;

namespace BookingManagement.Application.Consumers
{
	/// <summary>
	/// Consumes CancelBookingCommand from SAGA to cancel a booking when payment fails or times out.
	/// Delegates all domain logic to <see cref="BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers.CancelBookingCommandHandler"/>.
	/// </summary>
	public class CancelBookingCommandConsumer(
		IMediator mediator,
		ILoggingService<CancelBookingCommandConsumer> logger) : IConsumer<CancelBookingCommand>
	{
		public async Task Consume(ConsumeContext<CancelBookingCommand> context)
		{
			var message = context.Message;
			logger.LogInformation("Cancelling booking {BookingId} for SAGA {CorrelationId}. Reason: {Reason}",
				message.BookingId, message.CorrelationId, message.Reason);

			try
			{
				var appCommand = new CancelBookingCommand
				{
					CorrelationId = message.CorrelationId,
					BookingId = message.BookingId,
					Reason = message.Reason
				};

				await mediator.Send(appCommand, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error cancelling booking {BookingId}", message.BookingId);
				// Handler publishes BookingCancelledIntegrationEvent for both success and not-found paths.
				// Rethrow so MassTransit can retry / move to error queue.
				throw;
			}
		}
	}
}
