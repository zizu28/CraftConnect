using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;

namespace PaymentManagement.Domain.Entities
{
	public class Currency : AggregateRoot
	{
		public string Code { get; private set; } = string.Empty;
		public string Symbol { get; private set; } = string.Empty;
		public decimal ExchangeRate { get; private set; }
		public bool IsDefault { get; private set; }
		public CurrencyStatus CurrencyStatus { get; private set; }

		private Currency() { }

		public void SetAsDefault(Guid newCurrencyId, string newCurrencyCode)
		{
			IsDefault = true;
			AddIntegrationEvent(new DefaultCurrencyChanged(Id,newCurrencyId,newCurrencyCode));
		}

		public void UpdateRate(decimal newRate)
		{
			ExchangeRate = newRate;
		}

		public void Deactivate()
		{
			CurrencyStatus = CurrencyStatus.Inactive;
		}
	}
}
