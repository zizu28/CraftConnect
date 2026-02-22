using MediatR;

namespace NotificationManagement.Application.CQRS.Commands.NotificationCommands
{
	/// <summary>
	/// Application-layer command for sending a booking failure notification email.
	/// Mapped from the SharedKernel MassTransit message by the consumer.
	/// </summary>
	public class SendBookingFailureNotificationCommand : IRequest<Unit>
	{
		public Guid CorrelationId { get; set; }
		public Guid BookingId { get; set; }
		public Guid RecipientId { get; set; }
		public string CustomerEmail { get; set; } = string.Empty;
		public string? Reason { get; set; }
	}
}
