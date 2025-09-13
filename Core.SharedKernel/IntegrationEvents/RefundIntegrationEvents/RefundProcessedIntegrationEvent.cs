using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.RefundIntegrationEvents
{
	public record RefundProcessedIntegrationEvent(
		Guid RefundId,
		Guid PaymentId,
		Money RefundAmount,
		string Reason,
		Guid InitiatedBy) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
