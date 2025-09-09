using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record PaymentCompletedIntegrationEvent(
		Guid PaymentId,
		Guid? BookingId,
		Guid? OrderId,
		Guid? InvoiceId,
		Money Amount,
		Guid? PayerId,
		Guid? RecipientId) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
