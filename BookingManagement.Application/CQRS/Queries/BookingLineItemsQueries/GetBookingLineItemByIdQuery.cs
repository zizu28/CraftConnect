using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingLineItemsQueries
{
	public class GetBookingLineItemByIdQuery : IRequest<BookingLineItemResponseDTO>
	{
		public Guid Id { get; set; }
	}
}
