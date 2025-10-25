namespace CraftConnect.WebUI.ViewModels
{
	public class ServiceRequestItemVM
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string DateSubmitted { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty; // In Progress, Awaiting Proposals, etc.
	}
}
