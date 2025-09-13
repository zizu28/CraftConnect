using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingCompletedIntegrationEvent(
		Guid BookingId,
		Guid CustomerId,
		Guid CraftspersonId,
		decimal TotalAmount) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
