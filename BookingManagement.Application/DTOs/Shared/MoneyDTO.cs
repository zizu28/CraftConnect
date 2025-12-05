namespace BookingManagement.Application.DTOs.Shared
{
	public record MoneyDTO
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "USD";
	}
}
