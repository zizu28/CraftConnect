using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.IntegrationEvents
{
	public record BookingCancelledIntegrationEvent(
		Guid BookingId,
		CancellationReason? ReasonForCancellation) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();

		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
