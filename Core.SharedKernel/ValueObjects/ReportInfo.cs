using Core.SharedKernel.Enums;

namespace Core.SharedKernel.ValueObjects
{
	public record ReportInfo
	{
		public Guid ReportedItemId { get; private set; }
		public ReportType ReportType { get; private set; }
		public string Reason { get; private set; } = string.Empty;
		public DateTime Timestamp { get; private set; }

		private ReportInfo()
		{
			ReportedItemId = Guid.Empty;
			Reason = string.Empty;
			Timestamp = DateTime.MinValue;
		}

		public ReportInfo(Guid reportedItemId, ReportType reportType, string reason, DateTime timespamp)
		{
			ReportedItemId = reportedItemId;
			ReportType = reportType;
			Reason = reason;
			Timestamp = timespamp;
		}
	}
}
