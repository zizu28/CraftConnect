using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace PaymentManagement.Domain.Entities
{
	public class PaymentTransaction : Entity
	{
		public Guid PaymentId { get; private set; }
		public PaymentTransactionType Type { get; private set; }
		public Money Amount { get; private set; }
		public string Description { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public string? ExternalTransactionId { get; private set; }

		private PaymentTransaction() { }

		public static PaymentTransaction Create(
			Guid paymentId,
			PaymentTransactionType type,
			Money amount,
			string description,
			string? externalTransactionId = null)
		{
			return new PaymentTransaction
			{
				Id = Guid.NewGuid(),
				PaymentId = paymentId,
				Type = type,
				Amount = amount,
				Description = description,
				CreatedAt = DateTime.UtcNow,
				ExternalTransactionId = externalTransactionId
			};
		}
	}
}
