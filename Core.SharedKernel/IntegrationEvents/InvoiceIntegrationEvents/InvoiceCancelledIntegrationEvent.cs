using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.InvoiceIntegrationEvents
{
	public record InvoiceCancelledIntegrationEvent(
		Guid InvoiceId, 
		string InvoiceNumber, 
		Guid IssuedTo, 
		string Reason) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
