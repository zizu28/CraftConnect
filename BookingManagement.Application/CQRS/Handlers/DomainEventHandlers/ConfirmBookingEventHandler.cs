using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Events;
using Core.SharedKernel.IntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class ConfirmBookingEventHandler(
		IMessageBroker messageBroker,
		IBookingRepository bookingRepository) : IDomainEventHandler<BookingConfirmedEvent>
	{
		public async Task HandleAsync(BookingConfirmedEvent domainEvent, CancellationToken cancellationToken = default)
		{
			var booking = await bookingRepository.GetByIdAsync(domainEvent.BookingId, cancellationToken)
				?? throw new ArgumentNullException(nameof(domainEvent), "Booking not found.");
			var confirmBooking = new BookingConfirmedIntegrationEvent(
				domainEvent.BookingId,
				domainEvent.CustomerId,
				domainEvent.CraftmanId,
				booking.CalculateTotalPrice(),
				domainEvent.ConfirmedAt);
			
			await messageBroker.PublishAsync(confirmBooking, cancellationToken);
		}
	}
}
