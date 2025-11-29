namespace CraftConnect.WebUI.ViewModels
{
	public class PaymentHistoryItem
	{
		public Guid Id { get; set; }
		public string Project { get; set; }
		public string CraftsmanName { get; set; }
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; }
	}
}
