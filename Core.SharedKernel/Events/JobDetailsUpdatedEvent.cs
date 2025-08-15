using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record JobDetailsUpdatedEvent(
		Guid BookingId, Guid UpdatedByUserId, string NewDescription) : IDomainEvent
	{
		public Guid Id => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
