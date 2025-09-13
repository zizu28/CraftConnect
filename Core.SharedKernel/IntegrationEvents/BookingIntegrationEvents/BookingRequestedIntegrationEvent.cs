using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace Core.SharedKernel.IntegrationEvents.BookingIntegrationEvents
{
	public record BookingRequestedIntegrationEvent : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid BookingId { get; set; }
		public Guid CraftspersonId { get; set; }
		public Address ServiceAddress { get; set; }
		public string Description { get; set; }
		public GeoLocation Location { get; set; }
	}
}
