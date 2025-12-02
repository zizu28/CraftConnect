namespace CraftConnect.WASM.ViewModels
{
	public record BookingViewModel
	{
		public Guid CustomerId { get; set; }
		public Guid CraftmanId { get; set; }
		public string EmailAddress { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string InitialDescription { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public decimal Budget { get; set; }
		public List<string> DesiredSkills { get; set; } = [];
	}
}
