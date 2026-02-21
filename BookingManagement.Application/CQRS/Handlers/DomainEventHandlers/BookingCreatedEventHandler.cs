using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class BookingCreatedEventHandler(
		IMessageBroker messageBroker,
		IBookingRepository bookingRepository) : IDomainEventHandler<BookingRequestedIntegrationEvent>
	{
		public async Task HandleAsync(BookingRequestedIntegrationEvent domainEvent, CancellationToken cancellationToken = default)
		{
			if (domainEvent == null)
			{
				throw new ArgumentNullException(nameof(domainEvent), "Domain event cannot be null.");
			}
			var booking = await bookingRepository.GetByIdAsync(domainEvent.BookingId, cancellationToken);
			if(booking != null) 
			{
				throw new InvalidOperationException($"Booking with ID {domainEvent.BookingId} already exists.");
			}
			var newBooking = Booking.Create(domainEvent.CorrelationId, booking!.CustomerId, booking!.CraftmanId,
				booking!.ServiceAddress,domainEvent.Description, booking.StartDate, booking.EndDate);
			var bookingCreatedIntegrationEvent = new BookingRequestedIntegrationEvent(
				domainEvent.CorrelationId,
				newBooking.Id, newBooking.CraftmanId,
				newBooking.ServiceAddress, domainEvent.Description,
				CustomerId: newBooking.CustomerId,
				Amount: 0m,
				Currency: "NGN",
				CustomerEmail: string.Empty);
			await messageBroker.PublishAsync(bookingCreatedIntegrationEvent, cancellationToken);
		}
	}
}
