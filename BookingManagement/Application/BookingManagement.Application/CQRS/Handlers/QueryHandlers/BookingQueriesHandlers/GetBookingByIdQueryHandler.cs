using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers
{
	public class GetBookingByIdQueryHandler(
		IBookingRepository bookingRepository,
		IMapper mapper) : IRequestHandler<GetBookingByIdQuery, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
		{
			var booking = await bookingRepository.GetByIdAsync(request.Id, cancellationToken)
				?? throw new ApplicationException($"Booking with Id {request.Id} not found.");
			return mapper.Map<BookingResponseDTO>(booking);
		}
	}
}
