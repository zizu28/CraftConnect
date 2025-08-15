using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.Events
{
	public record UserRegisteredEvent(Guid UserId, Email Email, UserRole Role) : IDomainEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid Id => Guid.NewGuid();
	}
}
