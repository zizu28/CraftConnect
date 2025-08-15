using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record CraftmanSkillAddedEvent(Guid CraftmanId, string Name) : IDomainEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid Id => Guid.NewGuid();
	}
}
