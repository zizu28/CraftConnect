using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class UpdateBookingDetailsCommand : IRequest<string>
	{
		public Guid BookingId { get; set; }
		public BookingDetailsDTO BookingDetailsDTO { get; set; }
	}
}
