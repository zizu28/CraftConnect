using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record ProposalSubmitted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid ProposalId { get; }
		public Guid ProjectId { get; }
		public Guid CraftsmanId { get; }

		public ProposalSubmitted(Guid proposalId, Guid projectId, Guid craftsmanId)
		{
			ProposalId = proposalId;
			ProjectId = projectId;
			CraftsmanId = craftsmanId;
		}
	}
}
