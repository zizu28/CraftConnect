namespace CraftConnect.WebUI.ViewModels
{
	public class CustomerDashboardVM
	{
		public int ActiveServiceRequests { get; set; }
		public int NewProposals { get; set; }
		public int UnreadMessages { get; set; }
		public List<ServiceRequestVM> RecentServiceRequests { get; set; } = [];
		public List<CustomerProposalSummaryVM> IncomingProposals { get; set; } = [];
		public List<InboxMessageVM> InboxPreview { get; set; } = [];
	}
}
