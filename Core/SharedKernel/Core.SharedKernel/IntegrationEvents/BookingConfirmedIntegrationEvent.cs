using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents
{
	public record BookingConfirmedIntegrationEvent(
		Guid BookingId,
		Guid CustomerId,
		Guid CraftspersonId,
		decimal TotalAmount,
		DateTime ConfirmedAt) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
