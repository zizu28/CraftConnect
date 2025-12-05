using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingQueries
{
	public class GetBookingByIdQuery : IRequest<BookingResponseDTO>
	{
		public Guid Id { get; set; }
	}
}
