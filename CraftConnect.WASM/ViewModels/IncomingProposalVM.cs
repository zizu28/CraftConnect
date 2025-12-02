namespace CraftConnect.WASM.ViewModels
{
	public class IncomingProposalVM
	{
		public string Name { get; set; } = string.Empty;
		public decimal Rating { get; set; }
		public string ProposalSnippet { get; set; } = string.Empty;
		public string Price { get; set; } = string.Empty;
		public string AvatarUrl { get; set; } = string.Empty;
	}
}
