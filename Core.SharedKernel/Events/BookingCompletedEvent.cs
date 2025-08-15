using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record BookingCompletedEvent(Guid BookingId, DateTime CompletedAt) : IIntegrationEvent
	{
		public Guid Id => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
