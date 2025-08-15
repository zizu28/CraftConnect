using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class CreateBookingCommand : IRequest<BookingResponseDTO>
	{
		public BookingCreateDTO BookingDTO { get; set; }
	}
}
