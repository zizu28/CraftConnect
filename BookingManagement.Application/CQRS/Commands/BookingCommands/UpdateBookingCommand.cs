using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class UpdateBookingCommand : IRequest<BookingResponseDTO>
	{
		public BookingUpdateDTO BookingDTO { get; set; }
	}
}
