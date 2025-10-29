namespace CraftConnect.WebUI.ViewModels
{
	public class PendingPayout
	{
		public Guid Id { get; set; }
		public string ArtisanName { get; set; }
		public decimal Amount { get; set; }
		public DateTime ScheduledDate { get; set; }
		public string Status { get; set; } // "Pending", "In Progress", "Failed", "Paid"
		public bool IsSelected { get; set; }
	}
}
