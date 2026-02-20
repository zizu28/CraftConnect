using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Commands.BookingCommands;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MassTransit;

namespace BookingManagement.Application.Consumers
{
	/// <summary>
	/// Consumes ConfirmBookingCommand from SAGA to confirm a booking after payment
	/// </summary>
	public class ConfirmBookingCommandConsumer(
		IBookingRepository bookingRepository,
		IUnitOfWork unitOfWork,
		IMessageBroker publishEndpoint,
		ILoggingService<ConfirmBookingCommandConsumer> logger) : IConsumer<ConfirmBookingCommand>
	{
		public async Task Consume(ConsumeContext<ConfirmBookingCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Confirming booking {BookingId} for SAGA {CorrelationId}", 
				command.BookingId, command.CorrelationId);

			try
			{
				// Get the booking
				var booking = await bookingRepository.GetByIdAsync(command.BookingId, context.CancellationToken);
				if (booking == null)
				{
					logger.LogError(new Exception(), "Booking {BookingId} not found", command.BookingId);
					// Publish failure event
					await publishEndpoint.PublishAsync(new BookingCancelledIntegrationEvent(command.BookingId, command.Reason), context.CancellationToken);
					return;
				}

				// Confirm the booking (update status)
				booking.ConfirmBooking();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await bookingRepository.UpdateAsync(booking, context.CancellationToken);

					// Publish success event back to SAGA
					await publishEndpoint.PublishAsync(new BookingConfirmedIntegrationEvent(
						booking.Id,
						booking.CustomerId,
						booking.CraftmanId,
						booking.CalculateTotalPrice(),
						DateTime.UtcNow), context.CancellationToken);

					logger.LogInformation("Booking {BookingId} confirmed successfully", command.BookingId);
				}, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error confirming booking {BookingId}", command.BookingId);
				
				// Publish failure event
				await publishEndpoint.PublishAsync(new BookingCancelledIntegrationEvent(
					command.BookingId,
					command.Reason), context.CancellationToken);
			}
		}
	}
}
