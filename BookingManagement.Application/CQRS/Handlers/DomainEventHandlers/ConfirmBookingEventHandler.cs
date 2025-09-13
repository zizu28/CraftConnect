using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class ConfirmBookingEventHandler(
		IMessageBroker messageBroker,
		IBookingRepository bookingRepository) : IDomainEventHandler<BookingConfirmedIntegrationEvent>
	{
		public async Task HandleAsync(BookingConfirmedIntegrationEvent domainEvent, CancellationToken cancellationToken = default)
		{
			var booking = await bookingRepository.GetByIdAsync(domainEvent.BookingId, cancellationToken)
				?? throw new ArgumentNullException(nameof(domainEvent), "Booking not found.");
			var confirmBooking = new BookingConfirmedIntegrationEvent(
				domainEvent.BookingId,
				domainEvent.CustomerId, domainEvent.CustomerId, booking.CalculateTotalPrice(),
				domainEvent.ConfirmedAt);
			
			await messageBroker.PublishAsync(confirmBooking, cancellationToken);
		}
	}
}
