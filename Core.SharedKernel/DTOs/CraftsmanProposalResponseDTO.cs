using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs
{
	public class CraftsmanProposalResponseDTO
	{
		public Guid Id { get; set; }
		public Guid ProjectId { get; set; }
		public Guid CraftsmanId { get; set; }
		public string ClientName { get; set; } = string.Empty;
		public string ProjectTitle { get; set; } = string.Empty;
		public string CraftsmanAvatarUrl { get; set; } = string.Empty;
		public string CraftsmanName { get; set; } = string.Empty;
		public MoneyDTO Price { get; set; } = new();
		public string CoverLetter { get; set; } = string.Empty;
		public DateTimeRangeDTO ProposedTimeline { get; set; } = new();
		public DateTime SubmittedDate { get; set; }
		public ProposalStatus Status { get; set; }
		public string StatusName => Status.ToString();
	}
}
