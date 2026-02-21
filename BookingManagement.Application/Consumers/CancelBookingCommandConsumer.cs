using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Commands.BookingCommands;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MassTransit;

namespace BookingManagement.Application.Consumers
{
	/// <summary>
	/// Consumes CancelBookingCommand from SAGA to cancel a booking when payment fails
	/// </summary>
	public class CancelBookingCommandConsumer(
		IBookingRepository bookingRepository,
		IUnitOfWork unitOfWork,
		IMessageBroker publishEndpoint,
		ILoggingService<CancelBookingCommandConsumer> logger) : IConsumer<CancelBookingCommand>
	{
		public async Task Consume(ConsumeContext<CancelBookingCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Cancelling booking {BookingId} for SAGA {CorrelationId}. Reason: {Reason}", 
				command.BookingId, command.CorrelationId, command.Reason);

			try
			{
				// Get the booking
				var booking = await bookingRepository.GetByIdAsync(command.BookingId, context.CancellationToken);
				if (booking == null)
				{
					logger.LogWarning("Booking {BookingId} not found for cancellation", command.BookingId);
					// Still publish cancelled event (idempotent)
					await publishEndpoint.PublishAsync(new BookingCancelledIntegrationEvent(
						command.CorrelationId,
						command.BookingId,
						command.Reason), context.CancellationToken);
					return;
				}

				// Cancel the booking
				booking.CancelBooking(command.CorrelationId, CancellationReason.PaymentFailed);
				
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await bookingRepository.UpdateAsync(booking, context.CancellationToken);

					// Publish cancelled event back to SAGA
					await publishEndpoint.PublishAsync(new BookingCancelledIntegrationEvent(
						command.CorrelationId,
						booking.Id,
						command.Reason), context.CancellationToken);

					logger.LogInformation("Booking with ID {BookingId} cancelled successfully. Reason: {Reason}", 
						command.BookingId, command.Reason);
				}, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error cancelling booking {BookingId}", command.BookingId);
				
				// Still try to publish event for SAGA compensation
				try
				{
					await publishEndpoint.PublishAsync(new BookingCancelledIntegrationEvent(
						command.CorrelationId,
						command.BookingId,
						command.Reason), context.CancellationToken);
				}
				catch
				{
					// Swallow - SAGA will timeout and handle
				}
			}
		}
	}
}
