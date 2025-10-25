namespace CraftConnect.WebUI.ViewModels
{
	public class DashboardVM
	{
		public int ActiveProjects { get; set; }
		public int NewServiceRequests { get; set; }
		public int UnreadMessages { get; set; }
		public string MonthlyEarnings { get; set; } = string.Empty;
		public decimal TotalEarningsLast3Months { get; set; }
	}
}
