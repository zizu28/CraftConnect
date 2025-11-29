namespace CraftConnect.WebUI.ViewModels
{
	public class AdminSystemLog
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string EventType { get; set; }
		public string User { get; set; }
		public string Details { get; set; }
		public string IpAddress { get; set; }
	}
}
