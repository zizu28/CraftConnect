namespace Core.SharedKernel.Commands.NotificationCommands
{
	/// <summary>
	/// Command sent by SAGA to send booking failure notification
	/// </summary>
	public record SendBookingFailureNotificationCommand
	{
		/// <summary>
		/// Correlation ID for the SAGA instance
		/// </summary>
		public required Guid CorrelationId { get; init; }

		/// <summary>
		/// Booking that failed
		/// </summary>
		public required Guid BookingId { get; init; }

		/// <summary>
		/// Customer email
		/// </summary>
		public required string CustomerEmail { get; init; }

		/// <summary>
		/// Failure reason
		/// </summary>
		public string? Reason { get; init; }
	}
}
