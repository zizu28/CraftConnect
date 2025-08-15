using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingLineItemsQueries
{
	public class GetBookingLineItemByIdQuery : IRequest<BookingLineItemResponseDTO>
	{
		public Guid Id { get; set; }
	}
}
