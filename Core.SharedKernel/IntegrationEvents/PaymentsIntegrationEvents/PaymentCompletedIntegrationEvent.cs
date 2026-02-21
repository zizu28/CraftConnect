using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents
{
	public record PaymentCompletedIntegrationEvent(
		Guid CorrelationId,
		Guid PaymentId,
		Guid? BookingId,
		Guid? OrderId,
		Guid? InvoiceId,
		Money Amount,
		Guid? PayerId,
		Guid? RecipientId,
		Guid? SagaCorrelationId = null) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
