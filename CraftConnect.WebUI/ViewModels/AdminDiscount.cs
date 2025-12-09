using Core.SharedKernel.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class AdminDiscount
	{
		public Guid Id { get; set; }
		public bool IsSelected { get; set; }
		public string Code { get; set; }
		public DiscountType Type { get; set; }
		public decimal Value { get; set; }
		public int RedemptionCount { get; set; }
		public int RedemptionLimit { get; set; }
		public DateTime? ExpiryDate { get; set; }
		public DiscountStatus Status { get; set; }
	}
}
