using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.DomainEvents
{
	public record RefundIssued : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid InvoiceId { get; }
		public Guid ProjectId { get; }
		public decimal AmountRefunded { get; }
		public string Currency { get; }
		public string Reason { get; }

		public RefundIssued(Guid invoiceId, Guid projectId, decimal amountRefunded, string currency, string reason)
		{
			InvoiceId = invoiceId;
			ProjectId = projectId;
			AmountRefunded = amountRefunded;
			Currency = currency;
			Reason = reason;
		}
	}
}
