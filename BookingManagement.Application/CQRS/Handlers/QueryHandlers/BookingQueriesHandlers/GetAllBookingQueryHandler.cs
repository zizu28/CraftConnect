using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Application.DTOs.BookingDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers
{
	public class GetAllBookingQueryHandler(
		IBookingRepository bookingRepository,
		IMapper mapper) : IRequestHandler<GetAllBookingQuery, IEnumerable<BookingResponseDTO>>
	{
		public async Task<IEnumerable<BookingResponseDTO>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
		{
			var bookings = await bookingRepository.GetAllAsync(cancellationToken)
				?? throw new ArgumentNullException($"Could not retrieve list of bookings from database or cache.");
			return mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
		}
	}
}
