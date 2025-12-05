using Core.SharedKernel.Enums;

namespace Core.SharedKernel.DTOs
{
	public class PaymentTransactionResponseDTO
	{
		public Guid Id { get; set; }
		public string? Type { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public string? ExternalTransactionId { get; set; }
	}
}
