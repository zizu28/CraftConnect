namespace PaymentManagement.Application.DTOs.PaymentDTOs
{
	public class PaymentCreateDTO
	{
		public required decimal Amount { get; set; }

		public required string Currency { get; set; } = string.Empty;

		public required string PaymentMethod { get; set; }
		public required string PaymentStatus { get; set; }
		public required string PaymentType { get; set; }
		public required Guid PayerId { get; set; }

		public required Guid RecipientId { get; set; }

		public Guid BookingId { get; set; }
		public Guid OrderId { get; set; }
		public Guid InvoiceId { get; set; }

		public string? Description { get; set; }

		public required string BillingStreet { get; set; } = string.Empty;

		public required string BillingCity { get; set; } = string.Empty;

		public required string BillingPostalCode { get; set; } = string.Empty;

		//public string? BillingState { get; set; }

		//public string? BillingCountry { get; set; }

		public string? CardLast4Digits { get; set; }

		public string? CardBrand { get; set; }

		public DateTime? CardExpiryDate { get; set; }

		public string? ExternalMethodId { get; set; }
	}
}
