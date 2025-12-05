using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class UpdateBookingCommand : IRequest<BookingResponseDTO>
	{
		public BookingUpdateDTO BookingDTO { get; set; }
	}
}
