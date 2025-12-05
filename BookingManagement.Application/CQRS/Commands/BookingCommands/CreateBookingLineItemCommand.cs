using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.BookingCommands
{
	public class BookingLineItemCreateCommand : IRequest<BookingLineItemResponseDTO>
	{
		public Guid BookingId { get; set; }
		public BookingLineItemCreateDTO LineItemCreateDTO { get; set; }
	}
}
