using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Events;
using Core.SharedKernel.IntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class BookingCompleteEventHandler(
		IMessageBroker messageBroker,
		IBookingRepository bookingRepository) : IDomainEventHandler<BookingCompletedEvent>
	{
		public async Task HandleAsync(BookingCompletedEvent domainEvent, CancellationToken cancellationToken = default)
		{
			var booking = await bookingRepository.GetByIdAsync(domainEvent.BookingId, cancellationToken)
				?? throw new InvalidOperationException($"Booking with ID {domainEvent.BookingId} not found.");
			var bookingCompletedIntegrationEvent = new BookingCompletedIntegrationEvent(
				booking.Id, booking.CustomerId, booking.CraftmanId, booking.CalculateTotalPrice());
			await messageBroker.PublishAsync(bookingCompletedIntegrationEvent, cancellationToken);
		}
	}
}
