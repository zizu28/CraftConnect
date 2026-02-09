	namespace PaymentManagement.Application.DTOs
{
	/// <summary>
	/// Represents the webhook payload sent by Paystack for various events.
	/// See: https://paystack.com/docs/payments/webhooks
	/// </summary>
	public class PaystackWebhookPayload
	{
		/// <summary>
		/// The event type (e.g., "refund.processed", "charge.success", "refund.failed")
		/// </summary>
		public string Event { get; set; } = string.Empty;

		/// <summary>
		/// Event-specific data containing details about the transaction/refund
		/// </summary>
		public PaystackWebhookData Data { get; set; } = null!;
	}

	/// <summary>
	/// The data object contained in Paystack webhook payloads
	/// </summary>
	public class PaystackWebhookData
	{
		/// <summary>
		/// Unique identifier for the refund (integer from Paystack)
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Status of the refund: "pending", "success", "failed", "processing"
		/// </summary>
		public string Status { get; set; } = string.Empty;

		/// <summary>
		/// Refund amount in the smallest currency unit (kobo for NGN, cents for USD)
		/// </summary>
		public int Amount { get; set; }

		/// <summary>
		/// Currency code (e.g., "NGN", "USD", "GHS")
		/// </summary>
		public string Currency { get; set; } = string.Empty;

		/// <summary>
		/// Domain where the refund was processed ("live" or "test")
		/// </summary>
		public string Domain { get; set; } = string.Empty;

		/// <summary>
		/// Email of the person who initiated the refund
		/// </summary>
		public string? RefundedBy { get; set; }

		/// <summary>
		/// Merchant's note about the refund
		/// </summary>
		public string? MerchantNote { get; set; }

		/// <summary>
		/// Customer-facing note about the refund
		/// </summary>
		public string? CustomerNote { get; set; }

		/// <summary>
		/// Expected completion date/time of the refund
		/// </summary>
		public DateTime? ExpectedAt { get; set; }

		/// <summary>
		/// Amount deducted from merchant account
		/// </summary>
		public int DeductedAmount { get; set; }

		/// <summary>
		/// Whether the full amount was deducted
		/// </summary>
		public bool FullyDeducted { get; set; }

		/// <summary>
		/// Integration ID
		/// </summary>
		public int Integration { get; set; }

		/// <summary>
		/// Payment channel (if applicable)
		/// </summary>
		public string? Channel { get; set; }

		/// <summary>
		/// Transaction details associated with this refund
		/// </summary>
		public PaystackTransactionData? Transaction { get; set; }

		/// <summary>
		/// When the refund was created
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// When the refund was last updated
		/// </summary>
		public DateTime UpdatedAt { get; set; }
	}

	/// <summary>
	/// Transaction details within a webhook payload
	/// </summary>
	public class PaystackTransactionData
	{
		/// <summary>
		/// Transaction ID
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Domain where transaction occurred ("live" or "test")
		/// </summary>
		public string Domain { get; set; } = string.Empty;

		/// <summary>
		/// Unique transaction reference
		/// </summary>
		public string Reference { get; set; } = string.Empty;

		/// <summary>
		/// Transaction amount in smallest currency unit
		/// </summary>
		public int Amount { get; set; }

		/// <summary>
		/// When the transaction was paid
		/// </summary>
		public DateTime? PaidAt { get; set; }

		/// <summary>
		/// Payment channel used (e.g., "card", "bank", "ussd", "apple_pay")
		/// </summary>
		public string Channel { get; set; } = string.Empty;

		/// <summary>
		/// Currency code
		/// </summary>
		public string Currency { get; set; } = string.Empty;

		/// <summary>
		/// Authorization details (card info, etc.)
		/// </summary>
		public PaystackAuthorization? Authorization { get; set; }

		/// <summary>
		/// Customer details
		/// </summary>
		public PaystackCustomer? Customer { get; set; }
	}

	/// <summary>
	/// Authorization/payment method details
	/// </summary>
	public class PaystackAuthorization
	{
		/// <summary>
		/// Card expiry month
		/// </summary>
		public string? ExpMonth { get; set; }

		/// <summary>
		/// Card expiry year
		/// </summary>
		public string? ExpYear { get; set; }

		/// <summary>
		/// Account name on the payment method
		/// </summary>
		public string? AccountName { get; set; }

		/// <summary>
		/// Authorization code for reusable authorizations
		/// </summary>
		public string? AuthorizationCode { get; set; }

		/// <summary>
		/// Card type (e.g., "visa", "mastercard")
		/// </summary>
		public string? CardType { get; set; }

		/// <summary>
		/// Last 4 digits of card
		/// </summary>
		public string? Last4 { get; set; }

		/// <summary>
		/// Card bin (first 6 digits)
		/// </summary>
		public string? Bin { get; set; }

		/// <summary>
		/// Bank name
		/// </summary>
		public string? Bank { get; set; }
	}

	/// <summary>
	/// Customer information in webhook
	/// </summary>
	public class PaystackCustomer
	{
		/// <summary>
		/// Customer ID
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Customer email
		/// </summary>
		public string Email { get; set; } = string.Empty;

		/// <summary>
		/// Customer code
		/// </summary>
		public string? CustomerCode { get; set; }

		/// <summary>
		/// Phone number in international format
		/// </summary>
		public string? InternationalFormatPhone { get; set; }

		/// <summary>
		/// First name
		/// </summary>
		public string? FirstName { get; set; }

		/// <summary>
		/// Last name
		/// </summary>
		public string? LastName { get; set; }
	}
}
