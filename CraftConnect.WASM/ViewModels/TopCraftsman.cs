namespace CraftConnect.WASM.ViewModels
{
	public class TopCraftsman
	{
		public Guid Id { get; set; }
		public string AvatarUrl { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public int CompletedProjects { get; set; }
		public double Rating { get; set; }
	}
}
