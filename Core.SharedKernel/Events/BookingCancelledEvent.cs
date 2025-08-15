using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.Events
{
	public record BookingCancelledEvent(Guid BookingId, CancellationReason Reason) : IIntegrationEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid Id => Guid.NewGuid();
	}
}
