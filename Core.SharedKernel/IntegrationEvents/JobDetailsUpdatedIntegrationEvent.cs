using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents
{
	public record JobDetailsUpdatedIntegrationEvent(
		Guid BookingId, Guid UpdatedByUserId, string NewDescription) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
