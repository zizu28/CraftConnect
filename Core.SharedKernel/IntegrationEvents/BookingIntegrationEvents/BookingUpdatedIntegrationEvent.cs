using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingUpdatedIntegrationEvent(
		Guid BookingId,
		Guid CustomerId,
		Guid CraftsmanId,
		DateTime StartDate,
		DateTime EndDate,
		BookingStatus Status,
		decimal TotalPrice,
		DateTime UpdatedAt) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public TimeSpan Duration => StartDate - EndDate;
	}
}
