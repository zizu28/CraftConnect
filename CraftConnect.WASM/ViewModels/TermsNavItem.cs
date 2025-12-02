namespace CraftConnect.WASM.ViewModels
{
	public class TermsNavItem
	{
		public string Title { get; set; }
		public string Href { get; set; }
		public List<TermsNavItem> Children { get; set; } = [];
	}
}
