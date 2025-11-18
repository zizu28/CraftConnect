using Core.SharedKernel.Domain;

namespace PaymentManagement.Domain.Entities
{
	public class PaymentGateway : AggregateRoot
	{
		public string Name { get; private set; } = string.Empty;
		public string ApiKey { get; private set; } = string.Empty;
		public bool IsEnabled { get; private set; }
		
		private PaymentGateway() { }
		public void Enable()
		{
			IsEnabled = true;
		}

		public void Disable()
		{
			IsEnabled = false;
		}

		public void UpdateCredentials(string newApiKey)
		{
			ApiKey = newApiKey;
		}
	}
}
