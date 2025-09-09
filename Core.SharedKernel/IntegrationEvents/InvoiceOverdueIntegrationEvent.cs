using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record InvoiceOverdueIntegrationEvent(
		Guid InvoiceId, 
		string InvoiceNumber, 
		Guid IssuedTo, 
		Money TotalAmount, 
		DateTime DueDate) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
