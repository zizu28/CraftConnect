using Core.SharedKernel.Enums;
using PaymentManagement.Application.DTOs.PaymentTransactionDTOs;
using PaymentManagement.Application.DTOs.RefundDTOs;

namespace PaymentManagement.Application.DTOs.PaymentDTOs
{
	public class PaymentResponseDTO
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string AuthorizationUrl { get; set; } = string.Empty;
		public string PaymentMethod { get; set; } = string.Empty;
		public string? ExternalTransactionId { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? Description { get; set; }
		public Guid? InvoiceId { get; set; }
		public Guid? BookingId { get; set; }
		public Guid? OrderId { get; set; }
		public Guid PayerId { get; set; }
		public Guid RecipientId { get; set; }
		public string BillingStreet { get; set; } = string.Empty;
		public string BillingCity { get; set; } = string.Empty;
		public string BillingPostalCode { get; set; } = string.Empty;
		public string? BillingState { get; set; }
		public string? BillingCountry { get; set; }
		public string? CardLast4Digits { get; set; }
		public string? CardBrand { get; set; }
		public DateTime? CardExpiryDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ProcessedAt { get; set; }
		public DateTime? AuthorizedAt { get; set; }
		public DateTime? CapturedAt { get; set; }
		public string? FailureReason { get; set; }
		public decimal AvailableRefundAmount { get; set; }
		public bool CanBeRefunded { get; set; }
		public List<RefundResponseDTO> Refunds { get; set; } = [];
		public List<PaymentTransactionResponseDTO> Transactions { get; set; } = [];
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public List<string> Errors { get; set; } = [];
	}
}
