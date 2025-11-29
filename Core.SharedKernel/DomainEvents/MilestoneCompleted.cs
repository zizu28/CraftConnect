using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record MilestoneCompleted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid MilestoneId { get; }
		public Guid ProjectId { get; }
		public Guid CraftsmanId { get; }
		public string Title { get; }

		public MilestoneCompleted(Guid milestoneId, Guid projectId, Guid craftsmanId, string title)
		{
			MilestoneId = milestoneId;
			ProjectId = projectId;
			CraftsmanId = craftsmanId;
			Title = title;
		}
	}
}
