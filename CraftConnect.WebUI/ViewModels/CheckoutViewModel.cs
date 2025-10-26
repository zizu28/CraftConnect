namespace CraftConnect.WebUI.ViewModels
{
	public class CheckoutViewModel
	{
		public List<OrderSummaryItem> OrderItems { get; set; } = [];
		public decimal Total => OrderItems.Sum(item => item.Price);

		public string PaymentMethod { get; set; } = "CreditCard";

		// Payment Details
		public string CardNumber { get; set; }
		public string ExpirationDate { get; set; }
		public string Cvc { get; set; }
		public string NameOnCard { get; set; }

		// Billing Information
		public bool BillingSameAsRegistered { get; set; } = true;
		public string StreetAddress { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public string Country { get; set; } = "United States";
	}
}
