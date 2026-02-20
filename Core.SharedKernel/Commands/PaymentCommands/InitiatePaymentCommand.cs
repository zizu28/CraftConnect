namespace Core.SharedKernel.Commands.PaymentCommands
{
	/// <summary>
	/// Command sent by SAGA to initiate a payment for a booking
	/// </summary>
	public record InitiatePaymentCommand
	{
		/// <summary>
		/// Correlation ID for the SAGA instance
		/// </summary>
		public required Guid CorrelationId { get; init; }

		/// <summary>
		/// Booking that requires payment
		/// </summary>
		public required Guid BookingId { get; init; }

		/// <summary>
		/// Recipient that receives payment
		/// </summary>
		public required Guid RecipientId { get; init; }

		/// <summary>
		/// Customer making the payment
		/// </summary>
		public required Guid CustomerId { get; init; }

		/// <summary>
		/// Payment amount
		/// </summary>
		public required decimal Amount { get; init; }

		/// <summary>
		/// Currency code (e.g., USD, NGN)
		/// </summary>
		public required string Currency { get; init; }

		/// <summary>
		/// Customer email for payment receipt
		/// </summary>
		public required string CustomerEmail { get; init; }

		/// <summary>
		/// Payment description
		/// </summary>
		public required string Description { get; init; }

		/// <summary>
		/// Callback URL after payment
		/// </summary>
		public string? CallbackUrl { get; init; }
	}
}
