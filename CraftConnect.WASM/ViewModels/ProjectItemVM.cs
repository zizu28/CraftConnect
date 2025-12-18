namespace CraftConnect.WASM.ViewModels
{
	public class ProjectItemVM
	{
		public Guid ProjectId { get; set; }
		public string Project { get; set; } = string.Empty;
		public string Client { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string DueDate { get; set; } = string.Empty;
	}
}
