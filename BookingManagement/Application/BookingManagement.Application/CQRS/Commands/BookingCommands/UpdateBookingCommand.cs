using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class UpdateBookingCommand : IRequest<BookingResponseDTO>
	{
		public Guid BookingId { get; set; }
		public BookingUpdateDTO BookingDTO { get; set; }
	}
}
