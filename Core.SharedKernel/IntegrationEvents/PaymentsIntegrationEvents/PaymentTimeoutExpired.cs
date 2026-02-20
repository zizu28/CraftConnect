using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents
{
	public record PaymentTimeoutExpired : IIntegrationEvent
	{
		public Guid CorrelationId { get; init; }
		public Guid BookingId { get; init; }
		public Guid? PaymentId { get; init; }
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
