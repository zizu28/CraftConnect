using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace PaymentManagement.Domain.Entities
{
	public class Refund : Entity
	{
		public Guid PaymentId { get; private set; }
		public Money Amount { get; private set; }
		public string Reason { get; private set; }
		public RefundStatus Status { get; private set; }
		public Guid InitiatedBy { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public DateTime? ProcessedAt { get; private set; }
		public string? ExternalRefundId { get; private set; }

		private Refund() { }

		public static Refund Create(Guid paymentId, Money amount, string reason, Guid initiatedBy)
		{
			return new Refund
			{
				Id = Guid.NewGuid(),
				PaymentId = paymentId,
				Amount = amount,
				Reason = reason,
				InitiatedBy = initiatedBy,
				Status = RefundStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};
		}

		public void Complete(string externalRefundId)
		{
			if (Status != RefundStatus.Pending)
				throw new InvalidOperationException($"Cannot complete refund in {Status} status");

			Status = RefundStatus.Completed;
			ExternalRefundId = externalRefundId;
			ProcessedAt = DateTime.UtcNow;
		}

		public void Fail(string reason)
		{
			if (Status != RefundStatus.Pending)
				throw new InvalidOperationException($"Cannot fail refund in {Status} status");

			Status = RefundStatus.Failed;
			Reason = reason;
			ProcessedAt = DateTime.UtcNow;
		}
	}
}
