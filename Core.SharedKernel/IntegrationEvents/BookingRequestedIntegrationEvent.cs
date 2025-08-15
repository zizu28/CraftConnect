using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents
{
	public record BookingRequestedIntegrationEvent : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid BookingId { get; set; }
		public Guid CustomerId { get; set; }
		public Guid CraftspersonId { get; set; }
		public string JobDescription { get; set; }
		public string ServiceAddress { get; set; }
	}
}
