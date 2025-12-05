namespace Core.SharedKernel.DTOs
{
	public record MoneyDTO
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; } = "USD";
	}
}
