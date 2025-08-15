using Core.SharedKernel.Domain;

namespace Core.SharedKernel.Events
{
	public record BookingConfirmedEvent(
		Guid BookingId, Guid CraftmanId, Guid CustomerId, DateTime ConfirmedAt) : IIntegrationEvent
	{
		public decimal TotalAmount { get; set; }
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid Id => Guid.NewGuid();
	}
}
