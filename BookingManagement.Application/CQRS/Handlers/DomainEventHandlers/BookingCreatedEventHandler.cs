using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Core.EventServices;
using Core.SharedKernel.Domain;
using Core.SharedKernel.IntegrationEvents;

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
			var newBooking = Booking.Create(booking!.CustomerId, booking!.CraftmanId,
				booking!.ServiceAddress, booking.Details, booking.StartDate, booking.EndDate);
			var bookingCreatedIntegrationEvent = new BookingRequestedIntegrationEvent
			{
				BookingId = newBooking.Id,
				CraftspersonId = newBooking.CraftmanId,
				CustomerId = newBooking.CustomerId,
				ServiceAddress = $"{newBooking.ServiceAddress.Street}, {newBooking.ServiceAddress.City}, {newBooking.ServiceAddress.PostalCode}"
			};
			await messageBroker.PublishAsync(bookingCreatedIntegrationEvent, cancellationToken);
		}
	}
}
