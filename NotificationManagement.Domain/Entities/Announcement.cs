using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using System.Text.Json.Serialization;

namespace NotificationManagement.Domain.Entities
{
	public class Announcement : AggregateRoot
	{
		public string Title { get; private set; } = string.Empty;
		public string Content { get; private set; } = string.Empty;
		public DateTime PublicationDate { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public AnnouncementStatus AnnouncementStatus { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationAudience TargetAudience { get; private set; }

		private Announcement() { }

		public static Announcement CreateDraft(string title, string content, string targetAudience)
		{
			ArgumentException.ThrowIfNullOrEmpty(title);
			ArgumentException.ThrowIfNullOrEmpty(content);
			ArgumentException.ThrowIfNullOrEmpty(targetAudience);
			return new Announcement
			{
				Title = title,
				Content = content,
				TargetAudience = Enum.Parse<NotificationAudience>(targetAudience, true)
			};
		}

		public void Update(string title, string content, string targetAudience)
		{
			ArgumentException.ThrowIfNullOrEmpty(title);
			ArgumentException.ThrowIfNullOrEmpty(content);
			ArgumentException.ThrowIfNullOrEmpty(targetAudience);
			Title = title;
			Content = content;
			TargetAudience = Enum.Parse<NotificationAudience>(targetAudience, true);
		}

		public void Schedule(DateTime dateToPublish)
		{
			AnnouncementStatus = AnnouncementStatus.Scheduled;
			PublicationDate = dateToPublish;
		}

		public void Publish()
		{
			AnnouncementStatus = AnnouncementStatus.Published;
			PublicationDate = DateTime.UtcNow;
			AddIntegrationEvent(new AnnouncementPublished(Title, Content, TargetAudience.ToString()));
		}
	}
}
