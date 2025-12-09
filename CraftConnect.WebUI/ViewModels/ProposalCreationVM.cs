using Core.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraftConnect.WebUI.ViewModels
{
	public class ProposalCreationVM
	{
		public string Description { get; set; } = string.Empty;

		public RateType RateType { get; set; } = RateType.FixedPrice;

		[Column(TypeName = "decimal(18,2)")]
		public decimal ProposedCost { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		// Placeholder for file uploads (handling files is complex, this just tracks file names)
		public List<string> SupportingDocuments { get; set; } = [];
	}
}
