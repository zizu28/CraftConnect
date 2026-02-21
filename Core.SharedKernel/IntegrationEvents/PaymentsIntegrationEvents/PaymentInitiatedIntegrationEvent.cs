using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents
{
	public record PaymentInitiatedIntegrationEvent(
		Guid CorrelationId,
		Guid PaymentId,
		Guid? InvoiceId,
		Money Amount,
		Guid? PayerId,
		Guid? RecipientId,
		Guid? BookingId = null,
		Guid? SagaCorrelationId = null) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
