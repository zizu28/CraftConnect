using BookingManagement.Application.DTOs.Shared;
using Core.SharedKernel.Enums;

namespace BookingManagement.Application.DTOs.CraftmanProposalDTOs
{
	public class CraftsmanProposalResponseDTO
	{
		public Guid Id { get; set; }
		public Guid ProjectId { get; set; }
		public Guid CraftsmanId { get; set; }
		public string CraftsmanName { get; set; } = string.Empty;
		public MoneyDTO Price { get; set; } = new();
		public string CoverLetter { get; set; } = string.Empty;
		public DateTimeRangeDTO ProposedTimeline { get; set; } = new();
		public ProposalStatus Status { get; set; }
		public string StatusName => Status.ToString();
	}
}
