namespace CraftConnect.WebUI.ViewModels
{
	public class CustomerProjectSummaryVM
	{
		public int Id { get; set; }
		public string ProjectTitle { get; set; }
		public int ProposalCount { get; set; }
		public List<CustomerProposalVM> Proposals { get; set; } = new();
	}
}
