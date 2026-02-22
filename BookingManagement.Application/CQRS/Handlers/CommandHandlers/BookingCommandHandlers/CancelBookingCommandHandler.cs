using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.CommandHandlers.BookingCommandHandlers
{
	public class CancelBookingCommandHandler(
		IBookingRepository bookingRepository,
		ILoggingService<CancelBookingCommandHandler> logger,
		IUnitOfWork unitOfWork,
		IMessageBroker messageBroker) : IRequestHandler<CancelBookingCommand, Unit>
	{
		public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
		{
			var booking = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

			if (booking == null)
			{
				logger.LogWarning("Booking {BookingId} not found for cancellation — publishing cancelled event idempotently.", request.BookingId);
				// Publish idempotent cancelled event so SAGA does not get stuck
				await messageBroker.PublishAsync(
					new BookingCancelledIntegrationEvent(request.CorrelationId, request.BookingId, request.Reason),
					cancellationToken);
				return Unit.Value;
			}

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				// Domain method raises BookingCancelledIntegrationEvent internally
				booking.CancelBooking(request.CorrelationId, CancellationReason.PaymentFailed);

				var cancelledEvent = booking
					.DomainEvents
					.OfType<BookingCancelledIntegrationEvent>()
					.FirstOrDefault(e => e.BookingId == request.BookingId);

				await bookingRepository.UpdateAsync(booking, cancellationToken);

				if (cancelledEvent != null)
					await messageBroker.PublishAsync(cancelledEvent, cancellationToken);

				logger.LogInformation("Booking {BookingId} cancelled successfully. Reason: {Reason}",
					request.BookingId, request.Reason);
			}, cancellationToken);

			return Unit.Value;
		}
	}
}
