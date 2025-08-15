using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.Events
{
	public record UserRegisteredEvent(Guid UserId, Email Email, UserRole Role) : IIntegrationEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid EventId => Guid.NewGuid();
	}
}
