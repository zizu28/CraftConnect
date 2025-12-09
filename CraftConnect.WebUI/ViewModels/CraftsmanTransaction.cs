using Core.SharedKernel.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class CraftsmanTransaction
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public string Project { get; set; }
		public string Client { get; set; }
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; }
		public TransactionStatus Status { get; set; }
	}
}
