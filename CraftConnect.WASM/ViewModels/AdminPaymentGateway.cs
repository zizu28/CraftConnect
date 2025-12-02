namespace CraftConnect.WASM.ViewModels
{
	public class AdminPaymentGateway
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ApiKey { get; set; }
		public bool IsConfigured { get; set; }
		public bool IsEnabled { get; set; }
	}
}
