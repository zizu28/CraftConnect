using CraftConnect.WASM.Enums;
using System.ComponentModel.DataAnnotations;

namespace CraftConnect.WASM.ViewModels
{
	public class ProposalCreationVM
	{
		public Guid ProjectId { get; set; } // Needed for submission
		public string CoverLetter { get; set; } = string.Empty; // Maps to Description
		public decimal PriceAmount { get; set; }
		public string PriceCurrency { get; set; } = "USD"; // Default
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		// Removed RateType for now unless backend supports it
		// public RateType RateType { get; set; }
	}
}
