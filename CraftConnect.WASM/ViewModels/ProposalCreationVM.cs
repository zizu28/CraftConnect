using CraftConnect.WASM.Enums;
using System.ComponentModel.DataAnnotations;

namespace CraftConnect.WASM.ViewModels
{
	public class ProposalCreationVM
	{
		public string Description { get; set; } = string.Empty;

		public RateType RateType { get; set; } = RateType.FixedPrice;

		public decimal ProposedCost { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		// Placeholder for file uploads (handling files is complex, this just tracks file names)
		public List<string> SupportingDocuments { get; set; } = [];
	}
}
