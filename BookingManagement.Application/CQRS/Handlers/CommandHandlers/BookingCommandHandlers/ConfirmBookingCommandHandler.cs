using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class ConfirmBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<ConfirmBookingCommandHandler> logger,
		IMessageBroker messageBroker) : IRequestHandler<ConfirmBookingCommand, Unit>
	{
		public async Task<Unit> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
		{
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {request.BookingId} not found.");
			if (booking.Status != BookingStatus.Pending)
			{
				throw new InvalidOperationException($"Booking with ID {request.BookingId} is not in a pending state.");
			}
			booking.ConfirmBooking();
			await bookingRepository.UpdateAsync(booking, cancellationToken);

			var bookingConfirmedEvent = new BookingConfirmedIntegrationEvent(
				request.BookingId,
				request.CustomerId,
				request.CraftmanId,
				booking.CalculateTotalPrice(),
				request.ConfirmedAt);
			await messageBroker.PublishAsync(bookingConfirmedEvent, cancellationToken);
			await bookingRepository.SaveChangesAsync(cancellationToken);

			logger.LogInformation($"Booking with ID {request.BookingId} confirmed by Craftman with ID {request.CraftmanId} at {request.ConfirmedAt}.");
			booking.ClearEvents();
			logger.LogInformation($"Booking with ID {request.BookingId} confirmed successfully.");
			return Unit.Value;
		}
	}
}
