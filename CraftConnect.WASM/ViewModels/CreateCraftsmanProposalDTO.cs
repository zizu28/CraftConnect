using Core.SharedKernel.DTOs;

namespace CraftConnect.WASM.ViewModels
{
	public class CreateCraftsmanProposalDTO
	{
		public Guid ProjectId { get; set; }
		public MoneyDTO Price { get; set; } = new();
		public string CoverLetter { get; set; } = string.Empty;
		public DateTimeRangeDTO ProposedTimeline { get; set; } = new();
	}
}
