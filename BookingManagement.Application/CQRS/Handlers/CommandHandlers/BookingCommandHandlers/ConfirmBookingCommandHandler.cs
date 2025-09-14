using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class ConfirmBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<ConfirmBookingCommandHandler> logger,
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

				logger.LogInformation($"Booking with ID {request.BookingId} confirmed by Craftman with ID {request.CraftmanId} at {request.ConfirmedAt}.");
				logger.LogInformation($"Booking with ID {request.BookingId} confirmed successfully.");
			}, cancellationToken);
			
			return Unit.Value;
		}
	}
}
