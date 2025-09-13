using Core.SharedKernel.Domain;

namespace Core.SharedKernel.IntegrationEvents.ProductIntegrationEvents
{
	public record ProductCreatedIntegrationEvent : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public string ProductName { get; set; }
		public Guid ProductId { get; set; }
		public Guid CategoryId { get; set; }
		public Guid CraftmanId { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public bool IsActive { get; set; }
	}
}
