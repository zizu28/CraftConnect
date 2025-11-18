using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record ReviewSubmitted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid ReviewId { get; }
		public Guid ProjectId { get; }
		public Guid CustomerId { get; }
		public Guid CraftsmanId { get; }
		public int Rating { get; }
		public string Comment { get; }

		public ReviewSubmitted(Guid reviewId, Guid projectId, Guid customerId, Guid craftsmanId, int rating, string comment)
		{
			ReviewId = reviewId;
			ProjectId = projectId;
			CustomerId = customerId;
			CraftsmanId = craftsmanId;
			Rating = rating;
			Comment = comment;
		}
	}
}
