using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers
{
	public class GetBookingByIdQueryHandler(
		IBookingRepository bookingRepository,
		IMapper mapper) : IRequestHandler<GetBookingByIdQuery, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
		{
			var response = new BookingResponseDTO();
			if (request.Id == Guid.Empty)
			{
				response.IsSuccess = false;
				response.Message = "Invalid booking ID.";
				return response;
			}
			var booking = await bookingRepository.GetByIdAsync(request.Id, cancellationToken)
				?? throw new ApplicationException($"Booking with Id {request.Id} not found.");
			response = mapper.Map<BookingResponseDTO>(booking);
			response.IsSuccess = true;
			response.Message = "Booking retrieved successfully.";
			return response;
		}
	}
}
