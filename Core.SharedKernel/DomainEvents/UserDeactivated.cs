using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record UserDeactivated : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid UserId { get; }

		public UserDeactivated(Guid userId)
		{
			UserId = userId;
		}
	}
}
