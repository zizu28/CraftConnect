using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record BookingCompletedEvent(Guid BookingId, DateTime CompletedAt) : IDomainEvent
	{
		public Guid Id => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
