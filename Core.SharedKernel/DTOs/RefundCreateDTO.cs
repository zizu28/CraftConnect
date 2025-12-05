namespace Core.SharedKernel.DTOs
{
	public class RefundCreateDTO
	{
		public required Guid PaymentId { get; set; }

		public required decimal Amount { get; set; }

		public required string Currency { get; set; }
		public required string Reason { get; set; }
		public string Status { get; set; } = "Pending";
		public string ExternalRefundId { get; set; } = string.Empty;

		public required Guid InitiatedBy { get; set; }
	}
}
