using CraftConnect.WebUI.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class ProposalVM
	{
		public Guid Id { get; set; }
		public string ProjectTitle { get; set; } = string.Empty;
		public string ClientName { get; set; } = string.Empty;
		public DateTime SubmittedDate { get; set; }
		public ProposalStatus Status { get; set; }
		public string ProposedSolution { get; set; }
		public decimal Pricing { get; set; }
		public RateType PricingType { get; set; } = RateType.FixedPrice;
		public string EstimatedTimeline { get; set; } = string.Empty;
		public AttachedDocument Document { get; set; }
		public ProjectVM ProjectDetails { get; set; }
		public List<MessageVM> MessageHistory { get; set; }
	}
}
