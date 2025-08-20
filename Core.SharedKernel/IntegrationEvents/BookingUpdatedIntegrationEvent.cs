using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record BookingUpdatedIntegrationEvent(
		Guid BookingId,
		Guid CustomerId,
		Guid CraftsmanId,
		BookingStatus Status,
		DateTimeRange? Duration,
		decimal TotalPrice,
		DateTime UpdatedAt) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
