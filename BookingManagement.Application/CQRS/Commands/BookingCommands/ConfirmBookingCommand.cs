using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class ConfirmBookingCommand : IRequest<Unit>
	{
		public Guid BookingId { get; set; }
		public Guid CraftmanId { get; set; }
		public Guid CustomerId { get; set; }
		public DateTime ConfirmedAt { get; set; } = DateTime.UtcNow;
	}
}
