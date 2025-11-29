using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record AnnouncementPublished : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid AnnouncementId { get; }
		public string Title { get; }
		public string Content { get; }
		public string TargetAudience { get; }

		public AnnouncementPublished(string title, string content, string targetAudience)
		{
			Title = title;
			Content = content;
			TargetAudience = targetAudience;
		}
	}
}
