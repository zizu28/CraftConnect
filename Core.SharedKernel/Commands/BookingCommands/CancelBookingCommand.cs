namespace Core.SharedKernel.Commands.BookingCommands
{
	/// <summary>
	/// Command sent by SAGA to cancel a booking when payment fails or times out
	/// </summary>
	public record CancelBookingCommand
	{
		/// <summary>
		/// Correlation ID for the SAGA instance
		/// </summary>
		public required Guid CorrelationId { get; init; }

		/// <summary>
		/// Booking to cancel
		/// </summary>
		public required Guid BookingId { get; init; }

		/// <summary>
		/// Reason for cancellation
		/// </summary>
		public required string Reason { get; init; }
	}
}
