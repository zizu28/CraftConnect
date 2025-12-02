namespace CraftConnect.WASM.ViewModels
{
	public class RecentActivityItem
	{
		public string Id { get; set; }
		public string IconType { get; set; } // e.g., "user", "project", "milestone"
		public string Title { get; set; }
		public string Description { get; set; }
		public string TimeAgo { get; set; }
	}
}
