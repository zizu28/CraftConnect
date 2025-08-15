using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.Events
{
	public record BookingCreatedEvent(
		Guid BookingId, 
		Guid CustomerId, 
		Guid CraftmanId, 
		string Description, 
		Address ServiceAddress,
		DateTime CreatedAt) : IDomainEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid Id => Guid.NewGuid();
	}
}
