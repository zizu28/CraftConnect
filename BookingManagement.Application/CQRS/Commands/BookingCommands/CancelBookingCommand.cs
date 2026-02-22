using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class CancelBookingCommand : IRequest<Unit>
	{
		public Guid CorrelationId { get; set; }
		public Guid BookingId { get; set; }
		public string Reason { get; set; } = string.Empty;
	}
}
