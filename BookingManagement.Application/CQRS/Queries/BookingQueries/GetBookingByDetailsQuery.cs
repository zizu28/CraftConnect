using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingQueries
{
	public class GetBookingByDetailsQuery : IRequest<BookingResponseDTO>
	{
		public string Description { get; set; }
	}
}
