using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Events;
using Core.SharedKernel.IntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class BookingCancelledEventHandler(
		IBookingRepository bookingRepository,
		IMessageBroker messageBroker) : IDomainEventHandler<BookingCancelledEvent>
	{
		public async Task HandleAsync(BookingCancelledEvent domainEvent, CancellationToken cancellationToken = default)
		{
			var booking = await bookingRepository.GetByIdAsync(domainEvent.BookingId, cancellationToken)
				?? throw new ArgumentNullException(nameof(domainEvent));
			var bookingCancelledIntegrationEvent = new BookingCancelledIntegrationEvent(booking.Id,
				domainEvent.Reason);
			await messageBroker.PublishAsync(bookingCancelledIntegrationEvent, cancellationToken);
		}
	}
}
