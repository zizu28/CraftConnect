namespace CraftConnect.WASM.ViewModels
{
	public class CustomerProposalVM
	{
		public Guid Id { get; set; }
		public Guid ServiceRequestId { get; set; }
		public string CraftsmanName { get; set; }
		public string CraftsmanAvatarUrl { get; set; }
		public double Rating { get; set; }
		public int ReviewCount { get; set; }
		public string QuoteSummary { get; set; }
		public decimal Price { get; set; }
	}
}
