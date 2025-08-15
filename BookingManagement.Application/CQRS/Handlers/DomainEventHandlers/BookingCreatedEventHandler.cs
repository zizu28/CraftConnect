using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.Events;
using Core.SharedKernel.IntegrationEvents;

namespace BookingManagement.Application.CQRS.Handlers.DomainEventHandlers
{
	public class BookingCreatedEventHandler(
		IMessageBroker messageBroker,
		IBookingRepository bookingRepository) : IDomainEventHandler<BookingCreatedEvent>
	{
		public async Task HandleAsync(BookingCreatedEvent domainEvent, CancellationToken cancellationToken = default)
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
			var newBooking = Booking.Create(
				domainEvent.CustomerId,
				domainEvent.CraftmanId,
				domainEvent.ServiceAddress,
				domainEvent.Description
			);
			var bookingCreatedIntegrationEvent = new BookingRequestedIntegrationEvent
			{
				BookingId = domainEvent.BookingId,
				CraftspersonId = domainEvent.CraftmanId,
				CustomerId = domainEvent.CustomerId,
				JobDescription = newBooking!.Details.Description,
				ServiceAddress = $"{newBooking.ServiceAddress.Street}, {newBooking.ServiceAddress.City}, {newBooking.ServiceAddress.PostalCode}"
			};
			await messageBroker.PublishAsync(bookingCreatedIntegrationEvent, cancellationToken);
		}
	}
}
