using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class ConfirmBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<ConfirmBookingCommandHandler> logger,
		IMessageBroker messageBroker,
		IUnitOfWork unitOfWork) : IRequestHandler<ConfirmBookingCommand, Unit>
	{
		public async Task<Unit> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
		{
			await unitOfWork.ExecuteInTransactionAsync(async () =>
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

				logger.LogInformation($"Booking with ID {request.BookingId} confirmed by Craftman with ID {request.CraftmanId} at {request.ConfirmedAt}.");
				booking.ClearEvents();
				logger.LogInformation($"Booking with ID {request.BookingId} confirmed successfully.");
			}, cancellationToken);
			
			return Unit.Value;
		}
	}
}
