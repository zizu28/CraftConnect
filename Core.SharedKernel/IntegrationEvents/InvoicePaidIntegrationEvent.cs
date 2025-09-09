using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record InvoicePaidIntegrationEvent(
		Guid InvoiceId, 
		string InvoiceNumber, 
		Guid IssuedTo, 
		Guid PaymentId, 
		Money TotalAmount) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
