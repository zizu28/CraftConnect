using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingLineItemIntegrationEvent(
		Guid BookingId, Guid LineItemId, string Description,
		decimal Price, int Quantity) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
