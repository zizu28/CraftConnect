namespace Core.SharedKernel.Commands.PaymentCommands
{
	/// <summary>
	/// Command sent by SAGA to cancel a payment that timed out
	/// </summary>
	public record CancelPaymentCommand
	{
		/// <summary>
		/// Payment to cancel
		/// </summary>
		public required Guid PaymentId { get; init; }

		/// <summary>
		/// Reason for cancellation
		/// </summary>
		public required string Reason { get; init; }
		public Guid CorrelationId { get; init; }
	}
}
