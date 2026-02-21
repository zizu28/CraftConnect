using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingCancelledIntegrationEvent(
		Guid CorrelationId,
		Guid BookingId,
		string? ReasonForCancellation) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}	
}
