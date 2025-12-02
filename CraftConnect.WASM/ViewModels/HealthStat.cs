namespace CraftConnect.WASM.ViewModels
{
	public class HealthStat
	{
		public string Title { get; set; }
		public string Value { get; set; }
		public string Status { get; set; } // Healthy, Warning, Fast
		public string StatusMessage { get; set; }
	}
}
