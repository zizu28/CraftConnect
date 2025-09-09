using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record InvoicePartiallyPaidIntegrationEvent(
		Guid InvoiceId, 
		string InvoiceNumber, 
		Guid IssuedTo, 
		Guid PaymentId, 
		Money AmountPaid, 
		Money RemainingAmount) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
