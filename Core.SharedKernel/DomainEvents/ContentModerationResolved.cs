using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record ContentModerationResolved : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid ReportId { get; }
		public Guid ReportedItemId { get; }
		public string ReportType { get; }
		public string ActionTaken { get; } 
		public Guid ModeratorId { get; }

		public ContentModerationResolved(Guid reportId, Guid reportedItemId, string reportType,
			string actionTaken, Guid moderatorId)
		{
			ReportId = reportId;
			ReportedItemId = reportedItemId;
			ReportType = reportType;
			ActionTaken = actionTaken;
			ModeratorId = moderatorId;
		}
	}
}
