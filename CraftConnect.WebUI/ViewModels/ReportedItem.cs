using CraftConnect.WebUI.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class ReportedItem
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string ReporterInfo { get; set; }
		public DateTime Timestamp { get; set; }
		public ReportType Type { get; set; }
		public string Status { get; set; } // "Pending", "Resolved"
	}
}
