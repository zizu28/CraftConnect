using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record BookingLineItemAddedEvent(
		Guid BookingId, Guid LineItemId, string Description,
		decimal Price, int Quantity) : IIntegrationEvent
	{
		public Guid Id => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
