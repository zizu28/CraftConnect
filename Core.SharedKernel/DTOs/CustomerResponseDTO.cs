namespace Core.SharedKernel.DTOs
{
	public class CustomerResponseDTO
	{
		public Guid CustomerId { get; set; }
		public string? Email { get; set; } = string.Empty;
		public string? Phone { get; set; } = string.Empty;
		public string? Address { get; set; } = string.Empty;
		public string? PreferredPaymentMethod { get; set; } = "Cash";
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = string.Empty;
		public List<string> Errors { get; set; } = [];
	}

}
