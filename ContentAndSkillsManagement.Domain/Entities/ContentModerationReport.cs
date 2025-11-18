using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace ContentAndSkillsManagement.Domain.Entities
{
	public class ContentModerationReport : AggregateRoot
	{
		public Guid ReporterUserId { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public ReportStatus ReportStatus { get; private set; }
		public ReportInfo ReportInfo { get; private set; }
		public ContentPreview ContentPreview { get; private set; }
		private ModerationResolution? ModerationResolution { get; set; }

		private ContentModerationReport() { }

		public static ContentModerationReport File(Guid reporterId, Guid reportedItemId, 
			string type, string reason, ContentPreview content)
		{
			var contentModerationReport = new ContentModerationReport
			{
				ReporterUserId = reporterId,
				ReportStatus = ReportStatus.Pending,
				ReportInfo = new(reportedItemId, Enum.Parse<ReportType>(type, true),
				reason, DateTime.UtcNow),
				ContentPreview = content
			};
			return contentModerationReport;
		}

		public void Resolve(Guid moderatorId, string action, string notes)
		{
			if(ReportStatus == ReportStatus.Pending)
			{
				ReportStatus = ReportStatus.Resolved;
			}
			ModerationResolution = new ModerationResolution(moderatorId,
				Enum.Parse<ModerationAction>(action, true), notes, DateTime.UtcNow);
			AddIntegrationEvent(new ContentModerationResolved(Id,ReportInfo.ReportedItemId,ReportInfo.ReportType.ToString(), action, moderatorId));
		}
	}
}
