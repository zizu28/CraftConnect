using BookingManagement.Domain.Entities;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.BookingQueries
{
	public class GetBookingByDetailsQuery : IRequest<BookingResponseDTO>
	{
		public Guid BookingId { get; set; }
		public string Description { get; set; }
	}
}
