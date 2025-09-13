using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents
{
	public record PaymentRefundIntegrationEvent(
		Guid PaymentId,
		Money RefundAmount,
		string Reason,
		Guid PayerId,
		Guid RecipientId) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
