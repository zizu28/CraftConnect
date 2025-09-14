using Core.SharedKernel.Enums;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class DeleteBookingCommand : IRequest<Unit>
	{
		public Guid BookingId { get; set; }
		public string RecipientEmail { get; set; }
		public CancellationReason Reason { get; set; }
	}
}
