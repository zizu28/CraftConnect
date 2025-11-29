using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace PaymentManagement.Domain.Entities
{
	public class Payout : AggregateRoot
	{
		public Guid CraftsmanId { get; private set; }
		public Guid TransactionId { get; private set; }
		public Money Amount { get; private set; } = new(0, "");
		public PayoutStatus PayoutStatus { get; private set; }
		public List<Guid> InvoiceIds { get; private set; } = [];

		private Payout() { }

		public void Approve()
		{
			PayoutStatus = PayoutStatus.InProgress;
			AddIntegrationEvent(new PayoutApproved(Id, CraftsmanId, Amount.Amount));
		}

		public void Complete(Guid transactionId)
		{
			PayoutStatus = PayoutStatus.Paid;
			TransactionId = transactionId;
		}

		public void Failed(string reason)
		{
			PayoutStatus = PayoutStatus.Failed;
		}
	}
}
