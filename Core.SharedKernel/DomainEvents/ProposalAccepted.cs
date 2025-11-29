using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record ProposalAccepted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid ProposalId { get; }
		public Guid ProjectId { get; }
		public Guid CraftsmanId { get; }
		public Guid CustomerId { get; }
		public decimal Amount { get; }
		public string Currency { get; }

		public ProposalAccepted(Guid proposalId, Guid projectId, Guid craftsmanId, Guid customerId,
			decimal amount, string currency)
		{
			ProposalId = proposalId;
			ProjectId = projectId;
			CraftsmanId = craftsmanId;
			CustomerId = customerId;
			Amount = amount;
			Currency = currency;
		}
	}
}
