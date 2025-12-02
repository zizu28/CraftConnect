namespace CraftConnect.WASM.ViewModels
{
	public class ReviewViewModel
	{
		public int Rating { get; set; }
		public string Comments { get; set; } = string.Empty;
		public List<string> AppreciatedTags { get; set; } = [];
	}
}
