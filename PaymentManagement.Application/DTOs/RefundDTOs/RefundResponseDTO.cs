namespace PaymentManagement.Application.DTOs.RefundDTOs
{
	public class RefundResponseDTO
	{
		public Guid RefundId { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public string Reason { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public Guid InitiatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ProcessedAt { get; set; }
		public string? ExternalRefundId { get; set; }
	}
}
