namespace CraftConnect.WASM.ViewModels
{
	public record ProjectViewModel()
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string UserAvatarUrl { get; set; } = string.Empty;
		public List<string> Tags { get; set; } = new();
		public string BudgetRange { get; set; } = string.Empty;
		public string Deadline { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
	}
}
