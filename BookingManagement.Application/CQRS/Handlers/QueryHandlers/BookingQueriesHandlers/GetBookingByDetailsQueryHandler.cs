using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers
{
	public class GetBookingByDetailsQueryHandler(
		IBookingRepository bookingRepository,
		IMapper mapper) : IRequestHandler<GetBookingByDetailsQuery, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(GetBookingByDetailsQuery request, CancellationToken cancellationToken)
		{
			var booking = await bookingRepository.GetBookingByDetails(request.Description, cancellationToken)
				?? throw new ApplicationException("Booking with the specified details not found.");
			return mapper.Map<BookingResponseDTO>(booking);
		}
	}
}
