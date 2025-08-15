using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents
{
	public record CraftmanSkillIntegrationAddedEvent(Guid CraftmanId, string Name) : IIntegrationEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid EventId => Guid.NewGuid();
	}
}
