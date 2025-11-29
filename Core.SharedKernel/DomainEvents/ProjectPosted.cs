using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record ProjectPosted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid ProjectId { get; }
		public Guid CustomerId { get; }
		public string Title { get; }
		//public List<Guid> SkillIds { get; } // You can include this

		public ProjectPosted(Guid projectId, Guid customerId, string title)
		{
			ProjectId = projectId;
			CustomerId = customerId;
			Title = title;
		}
	}
}
