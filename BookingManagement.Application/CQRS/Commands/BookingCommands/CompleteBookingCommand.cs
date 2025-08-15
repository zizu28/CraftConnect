using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class CompleteBookingCommand : IRequest<Unit>
	{
		public Guid BookingId { get; set; }
		public DateTime CompletedAt { get; set; }
	}
}
