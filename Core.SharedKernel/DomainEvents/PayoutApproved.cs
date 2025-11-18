using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record PayoutApproved : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid PayoutId { get; }
		public Guid CraftsmanId { get; }
		public decimal Amount { get; }

		public PayoutApproved(Guid payoutId, Guid craftsmanId, decimal amount)
		{
			PayoutId = payoutId;
			CraftsmanId = craftsmanId;
			Amount = amount;
		}
	}
}
