using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingQueries
{
	public class GetAllBookingQuery : IRequest<IEnumerable<BookingResponseDTO>>
	{
	}
}
