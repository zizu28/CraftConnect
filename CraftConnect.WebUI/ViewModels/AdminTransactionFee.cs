using CraftConnect.WebUI.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class AdminTransactionFee
	{
		public Guid Id { get; set; }
		public string RuleName { get; set; }
		public DiscountType FeeType { get; set; } = DiscountType.FixedAmount;
		public decimal FeeValue { get; set; }
		public string AppliesTo { get; set; }
		public UserStatus Status { get; set; } // "Active" or "Inactive"
	}
}
