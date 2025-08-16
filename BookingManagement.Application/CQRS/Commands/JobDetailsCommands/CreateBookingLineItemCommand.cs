using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.JobDetailsCommands
{
	public class BookingLineItemCreateCommand : IRequest<BookingLineItemResponseDTO>
	{
		public Guid BookingId { get; set; }
		public BookingLineItemCreateDTO LineItemCreateDTO { get; set; }
	}
}
