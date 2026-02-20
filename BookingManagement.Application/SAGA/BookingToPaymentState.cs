using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace BookingManagement.Application.SAGA
{
	public class BookingToPaymentState : SagaStateMachineInstance
	{
		public Guid CorrelationId { get ; set ; }
		public  string CurrentState { get; set; }
		public Guid BookingId { get; set; }
		public Guid PaymentId { get; set; }
		public Guid? PaymentTimeoutTokenId { get; set; }
		public Guid? BookingConfirmationTimeoutTokenId { get; set; }
		public Guid CustomerId { get; set; }
		public Guid CraftsmanId { get; set; }
		public Guid InvoiceId { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; }
		public string? PaymentAuthorizationUrl { get; set; }
		public string? PaymentReference { get; set; }
		public string CustomerEmail { get; set; } = string.Empty;
		public string ServiceDescription { get; set; } = string.Empty;
		public DateTime? ScheduledDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? PaymentInitiatedAt { get; set; }
		public DateTime? PaymentCompletedAt { get; set; }
		public DateTime? BookingConfirmedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
		public DateTime? CancelledAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string? FailureReason { get; set; }
		public int PaymentRetryCount { get; set; }
		public int BookingConfirmationRetryCount { get; set; }
		public bool CompensationCompleted { get; set; }
		public DateTime? PaymentTimeoutExpiresAt { get; set; }
		[Timestamp]
		public byte[] RowVersion { get; set; } = [];
		public string? Metadata { get; set; }
	}
}
