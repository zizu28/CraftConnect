using Core.SharedKernel.Enums;

namespace CraftConnect.WASM.ViewModels
{
	public class ServiceRequestVM
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public Guid Id { get; set; }
		public string ProjectTitle { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public DateTime DateSubmitted { get; set; }
		public ServiceRequestStatus Status { get; set; }
		public string ProjectDescription { get; set; }
		public List<string> SkillsRequired { get; set; } = new();
		public decimal Budget { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public List<AttachedDocument> Attachments { get; set; } = new();
		public List<CustomerProposalVM> Proposals { get; set; } = new();
	}
}
