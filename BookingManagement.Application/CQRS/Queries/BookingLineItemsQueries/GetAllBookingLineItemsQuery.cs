using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingLineItemsQueries
{
	public class GetAllBookingLineItemsQuery : IRequest<IEnumerable<BookingLineItemResponseDTO>>
	{
	}
}
