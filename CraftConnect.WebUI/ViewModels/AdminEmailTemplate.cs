namespace CraftConnect.WebUI.ViewModels
{
	public class AdminEmailTemplate
	{
		public Guid Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string Icon { get; set; } // SVG path or icon name
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
