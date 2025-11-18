using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record InvoicePaid : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid InvoiceId { get; }
		public Guid ProjectId { get; }
		public Guid CraftsmanId { get; }
		public Guid CustomerId { get; }
		public decimal AmountPaid { get; }

		public InvoicePaid(Guid invoiceId, Guid projectId, Guid craftsmanId, Guid customerId,
			decimal amountPaid)
		{
			InvoiceId = invoiceId;
			ProjectId = projectId;
			CraftsmanId = craftsmanId;
			CustomerId = customerId;
			AmountPaid = amountPaid;
		}
	}
}
