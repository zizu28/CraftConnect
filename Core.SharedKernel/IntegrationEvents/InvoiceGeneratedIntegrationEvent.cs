using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record InvoiceGeneratedIntegrationEvent(
		Guid InvoiceId, 
		string InvoiceNumber, 
		Guid IssuedTo, 
		Guid IssuedBy, 
		Money TotalAmount, 
		DateTime DueDate,
		Guid? BookingId,
		Guid? OrderId) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
