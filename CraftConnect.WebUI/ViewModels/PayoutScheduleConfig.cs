namespace CraftConnect.WebUI.ViewModels
{
	public class PayoutScheduleConfig
	{
		public string Frequency { get; set; } = "Weekly";
		public decimal MinimumThreshold { get; set; } = 100.00m;
	}
}
