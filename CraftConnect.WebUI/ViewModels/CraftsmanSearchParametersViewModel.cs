namespace CraftConnect.WebUI.ViewModels
{
	public class CraftsmanSearchParametersViewModel
	{
		public string Query { get; set; } = string.Empty;
		public string Skill { get; set; } = "All Skills";
		public string Location { get; set; } = "All Locations";
		public string Rating { get; set; } = "Any Rating";
		public string Availability { get; set; } = "Any Time";
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 9;
	}
}
