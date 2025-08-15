using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record JobDetailsUpdatedEvent(
		Guid BookingId, Guid UpdatedByUserId, string NewDescription) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
