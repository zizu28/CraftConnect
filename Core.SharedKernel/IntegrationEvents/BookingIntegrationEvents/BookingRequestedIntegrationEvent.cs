using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingRequestedIntegrationEvent(
		Guid CorrelationId,
		Guid BookingId,
		Guid CraftspersonId,
		Address ServiceAddress,
		string Description,
		Guid CustomerId,
		decimal Amount,
		string Currency,
		string CustomerEmail) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
