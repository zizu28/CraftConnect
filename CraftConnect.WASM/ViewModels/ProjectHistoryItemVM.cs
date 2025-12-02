namespace CraftConnect.WASM.ViewModels
{
	public class ProjectHistoryItemVM
	{
		public Guid Id { get; set; }
		public string ProjectTitle { get; set; }
		public string CraftsmanName { get; set; }
		public string CraftsmanAvatarUrl { get; set; }
		public DateTime CompletedDate { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
	}
}
