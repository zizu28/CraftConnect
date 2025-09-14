using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingRequestedIntegrationEvent(
		Guid BookingId,
		Guid CraftspersonId,
		Address ServiceAddress,
		string Description) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
