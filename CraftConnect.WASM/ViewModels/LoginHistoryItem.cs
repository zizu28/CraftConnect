namespace CraftConnect.WASM.ViewModels
{
	public class LoginHistoryItem
	{
		public int Id { get; set; }
		public string Device { get; set; }
		public string DeviceIcon { get; set; } // e.g., "desktop", "tablet", "mobile"
		public string Location { get; set; }
		public DateTime Time { get; set; }
	}
}
