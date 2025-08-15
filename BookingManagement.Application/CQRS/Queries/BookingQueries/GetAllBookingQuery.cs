using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingQueries
{
	public class GetAllBookingQuery : IRequest<IEnumerable<BookingResponseDTO>>
	{
	}
}
