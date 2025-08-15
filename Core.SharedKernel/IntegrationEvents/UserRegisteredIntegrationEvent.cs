using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record UserRegisteredIntegrationEvent(Guid UserId, Email Email, UserRole Role) : IIntegrationEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid EventId => Guid.NewGuid();
	}
}
