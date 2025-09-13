using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents
{
	public record PaymentFailedIntegrationEvent(
		Guid PaymentId,
		Guid? BookingId, 
		Guid? OrderId, 
		Guid? InvoiceId, 
		string Reason, 
		Guid? PayerId, 
		Guid? RecipientId) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
