namespace Core.SharedKernel.Commands.NotificationCommands
{
	/// <summary>
	/// Command sent by SAGA to send booking confirmation notification
	/// </summary>
	public record SendBookingConfirmationNotificationCommand
	{
		/// <summary>
		/// Correlation ID for the SAGA instance
		/// </summary>
		public required Guid CorrelationId { get; init; }

		/// <summary>
		/// Booking that was confirmed
		/// </summary>
		public required Guid BookingId { get; init; }

		/// <summary>
		/// Booking that was confirmed
		/// </summary>
		public required Guid RecipientId { get; init; }

		/// <summary>
		/// Customer email
		/// </summary>
		public required string CustomerEmail { get; init; }

		/// <summary>
		/// Service description
		/// </summary>
		public required string ServiceDescription { get; init; }

		/// <summary>
		/// Scheduled appointment date
		/// </summary>
		public DateTime? ScheduledDate { get; init; }

		/// <summary>
		/// Payment amount
		/// </summary>
		public required decimal Amount { get; init; }

		/// <summary>
		/// Currency
		/// </summary>
		public required string Currency { get; init; }

		/// <summary>
		/// Payment reference
		/// </summary>
		public string? PaymentReference { get; init; }
	}
}
