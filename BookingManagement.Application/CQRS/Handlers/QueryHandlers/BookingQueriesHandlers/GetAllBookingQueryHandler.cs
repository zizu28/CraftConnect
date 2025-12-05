using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using Core.SharedKernel.DTOs;
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
				?? throw new ArgumentNullException(nameof(request), $"Could not retrieve list of bookings from database or cache.");
			var responses = mapper.Map<IEnumerable<BookingResponseDTO>>(bookings);
			foreach(var response in responses)
			{
				response.Message = $"Booking with ID {response.BookingId} succesfully retrieved.";
				response.IsSuccess = true;
			}
			return responses;
		}
	}
}
