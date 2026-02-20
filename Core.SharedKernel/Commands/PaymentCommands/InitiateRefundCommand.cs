namespace Core.SharedKernel.Commands.PaymentCommands
{
	/// <summary>
	/// Command sent by SAGA to initiate a refund when booking confirmation fails
	/// </summary>
	public record InitiateRefundCommand
	{
		/// <summary>
		/// Correlation ID for the SAGA instance
		/// </summary>
		public required Guid CorrelationId { get; init; }

		/// <summary>
		/// Payment to refund
		/// </summary>
		public required Guid PaymentId { get; init; }

		/// <summary>
		/// Refund amount
		/// </summary>
		public required decimal Amount { get; init; }

		/// <summary>
		/// Refund currency (e.g., USD)
		/// </summary>
		public required string Currency { get; init; }

		/// <summary>
		/// Recipient email for refund notification
		/// </summary>
		public required string RecipientEmail { get; init; }

		/// <summary>
		/// Reason for refund
		/// </summary>
		public required string Reason { get; init; }
	}
}
