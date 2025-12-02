namespace CraftConnect.WASM.ViewModels
{
	public class PayoutScheduleConfig
	{
		public string Frequency { get; set; } = "Weekly";
		public decimal MinimumThreshold { get; set; } = 100.00m;
	}
}
