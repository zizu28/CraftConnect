namespace CraftConnect.WebUI.ViewModels
{
	public class ProjectVM
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Category { get; set; }
		public DateTime PostedDate { get; set; }
		public string OriginalListingUrl { get; set; } = "https://localhost:7284/projects/available";
		public ClientVM Client { get; set; }
	}
}
