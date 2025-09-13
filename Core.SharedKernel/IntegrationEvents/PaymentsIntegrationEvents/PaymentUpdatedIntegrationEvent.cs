using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents
{
	public record PaymentUpdatedIntegrationEvent(
		Guid PaymentId,
		string? Description,
		Address? BillingAddress,
		PaymentStatus? PaymentStatus,
		string? ExternalTransactionId,
		string? PaymentIntentId,
		string? FailureReason) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
