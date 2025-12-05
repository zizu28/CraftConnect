using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers
{
	public class GetBookingByDetailsQueryHandler(
		IBookingRepository bookingRepository,
		IMapper mapper) : IRequestHandler<GetBookingByDetailsQuery, BookingResponseDTO>
	{
		public async Task<BookingResponseDTO> Handle(GetBookingByDetailsQuery request, CancellationToken cancellationToken)
		{
			var details = new JobDetails(request.BookingId, request.Description);
			var booking = await bookingRepository.GetBookingByDetails(details, cancellationToken)
				?? throw new ApplicationException("Booking with the specified details not found.");
			var response = mapper.Map<BookingResponseDTO>(booking);
			response.Message = $"Booking with ID {response.BookingId} succesfully retrieved.";
			response.IsSuccess = true;
			return response;
		}
	}
}
