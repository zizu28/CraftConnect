using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using System.ComponentModel.DataAnnotations;

namespace ReviewRatingManagement.Domain.Entities
{
	public class Review : AggregateRoot
	{
		public Guid ProjectId { get; private set; }
		public Guid CustomerId { get; private set; }
		public Guid CraftsmanId { get; private set; }
		public int Rating { get; private set; }
		[MaxLength(250, ErrorMessage = "Comment cannot exceed 250 characters.")]
		public string Comment { get; private set; } = string.Empty;
		private List<string> Tags { get; set; } = [];
		private List<Guid> ImageIds { get; set; } = [];
		public DateTime Timestamp { get; private set; }

		private Review() { }

		public static Review Create(Guid projectId, Guid customerId, Guid craftsmanId, 
			int rating, string comment, List<string> tags)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(rating, 5);
			ArgumentOutOfRangeException.ThrowIfLessThan(rating, 1);
			// Check that projectId is in "Completed" status and that customerId is the owner
			
			var review = new Review
			{
				ProjectId = projectId,
				CustomerId = customerId,
				CraftsmanId = craftsmanId,
				Rating = rating,
				Comment = comment,
				Tags = tags
			};
			review.AddIntegrationEvent(new ReviewSubmitted(review.Id, projectId, customerId, craftsmanId,
				rating, comment));
			return review;
		}

		public void AddImage(Guid documentId)
		{
			ImageIds.Add(documentId);
		}
	}
}
