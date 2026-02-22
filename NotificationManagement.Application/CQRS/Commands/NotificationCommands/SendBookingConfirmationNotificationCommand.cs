using MediatR;

namespace NotificationManagement.Application.CQRS.Commands.NotificationCommands
{
	/// <summary>
	/// Application-layer command for sending a booking confirmation notification email.
	/// Mapped from the SharedKernel MassTransit message by the consumer.
	/// </summary>
	public class SendBookingConfirmationNotificationCommand : IRequest<Unit>
	{
		public Guid CorrelationId { get; set; }
		public Guid BookingId { get; set; }
		public Guid RecipientId { get; set; }
		public string CustomerEmail { get; set; } = string.Empty;
		public string ServiceDescription { get; set; } = string.Empty;
		public DateTime? ScheduledDate { get; set; }
		public decimal Amount { get; set; }
		public string Currency { get; set; } = string.Empty;
		public string? PaymentReference { get; set; }
	}
}
