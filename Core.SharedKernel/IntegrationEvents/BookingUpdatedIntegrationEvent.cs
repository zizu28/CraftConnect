using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using NodaTime;

namespace Core.SharedKernel.IntegrationEvents
{
	public record BookingUpdatedIntegrationEvent(
		Guid BookingId,
		Guid CustomerId,
		Guid CraftsmanId,
		LocalDateTime StartDate,
		LocalDateTime EndDate,
		BookingStatus Status,
		decimal TotalPrice,
		LocalDateTime UpdatedAt) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Period Duration => Period.Between(StartDate, EndDate);
	}
}
