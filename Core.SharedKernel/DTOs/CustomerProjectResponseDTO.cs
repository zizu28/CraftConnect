using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs
{
	public class CustomerProjectResponseDTO
	{
		public Guid Id { get; set; }
		public Guid CustomerId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string CustomerAvatarUrl { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public ServiceRequestStatus Status { get; set; }
		public string StatusName => Status.ToString();
		public MoneyDTO Budget { get; set; } = new();
		public DateTimeRangeDTO Timeline { get; set; } = new();
		public Guid? SelectedCraftsmanId { get; set; }
		public Guid? SelectedProposalId { get; set; }
		public List<SkillDTO> Skills { get; set; } = [];
		public List<Guid> MilestoneIds { get; set; } = [];
		public List<Guid> AttachmentIds { get; set; } = [];
		public List<Guid> ProposalIds { get; set; } = [];
		public int ProposalCount { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
