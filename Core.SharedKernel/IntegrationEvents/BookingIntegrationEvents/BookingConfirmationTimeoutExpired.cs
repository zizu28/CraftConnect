using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingConfirmationTimeoutExpired : IIntegrationEvent
	{
		public Guid CorrelationId { get; init; }
		public Guid BookingId { get; init; }
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
