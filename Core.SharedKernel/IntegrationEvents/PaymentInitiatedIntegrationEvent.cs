using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents
{
	public record PaymentInitiatedIntegrationEvent(
		Guid PaymentId,
		Guid? InvoiceId,
		Money Amount,
		Guid? PayerId,
		Guid? RecipientId) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
