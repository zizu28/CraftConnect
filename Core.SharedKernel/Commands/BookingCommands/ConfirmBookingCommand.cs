namespace Core.SharedKernel.Commands.BookingCommands
{
	/// <summary>
	/// Command sent by SAGA to confirm a booking after payment is completed
	/// </summary>
	public record ConfirmBookingCommand
	{
		/// <summary>
		/// Correlation ID for the SAGA instance
		/// </summary>
		public required Guid CorrelationId { get; init; }

		/// <summary>
		/// Booking to confirm
		/// </summary>
		public required Guid BookingId { get; init; }

		/// <summary>
		/// Payment that was completed
		/// </summary>
		public Guid? PaymentId { get; init; }

		/// <summary>
		/// Payment reference from payment gateway
		/// </summary>
		public string? PaymentReference { get; init; }

		/// <summary>
		/// Payment cancellation reason
		/// </summary>
		public string? Reason { get; init; }
	}
}
