namespace CraftConnect.WASM.ViewModels
{
	public class AdminCurrency
	{
		public Guid Id { get; set; }
		public bool IsDefault { get; set; }
		public string CurrencyName { get; set; }
		public string Code { get; set; }
		public string Symbol { get; set; }
		public decimal ExchangeRate { get; set; }
		public string Status { get; set; } // "Active" or "Inactive"
	}
}
