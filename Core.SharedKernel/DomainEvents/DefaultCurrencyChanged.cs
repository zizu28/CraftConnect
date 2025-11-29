using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record DefaultCurrencyChanged : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid OldCurrencyId { get; }
		public Guid NewCurrencyId { get; }
		public string NewCurrencyCode { get; } // e.g., "USD"

		public DefaultCurrencyChanged(Guid oldCurrencyId, Guid newCurrencyId, string newCurrencyCode)
		{
			OldCurrencyId = oldCurrencyId;
			NewCurrencyId = newCurrencyId;
			NewCurrencyCode = newCurrencyCode;
		}
	}
}
