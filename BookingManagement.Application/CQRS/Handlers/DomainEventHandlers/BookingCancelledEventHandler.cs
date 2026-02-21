using BookingManagement.Application.Contracts;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class BookingCancelledEventHandler(
		IBookingRepository bookingRepository,
		IMessageBroker messageBroker) : IDomainEventHandler<BookingCancelledIntegrationEvent>
	{
		public async Task HandleAsync(BookingCancelledIntegrationEvent domainEvent, CancellationToken cancellationToken = default)
		{
			var booking = await bookingRepository.GetByIdAsync(domainEvent.BookingId, cancellationToken)
				?? throw new ArgumentNullException(nameof(domainEvent));
			var bookingCancelledIntegrationEvent = new BookingCancelledIntegrationEvent(domainEvent.CorrelationId, booking.Id, CancellationReason.Other.ToString());
			await messageBroker.PublishAsync(bookingCancelledIntegrationEvent, cancellationToken);
		}
	}
}
