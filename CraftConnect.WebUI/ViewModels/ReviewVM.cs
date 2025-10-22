namespace CraftConnect.WebUI.ViewModels
{
	public class ReviewVM
	{
		public string ReviewerName { get; set; } = string.Empty;
		public decimal Rating { get; set; }
		public string Comment { get; set; } = string.Empty;
		public DateTime DatePosted { get; set; }
	}
}
