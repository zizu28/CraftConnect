using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.DTOs;
using Infrastructure.Cache;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers
{
	public class GetBookingByIdQueryHandler(
		IBookingRepository bookingRepository,
		IMapper mapper,
		ICacheService cacheService) : IRequestHandler<GetBookingByIdQuery, BookingResponseDTO>
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

			var booking = await cacheService.GetOrCreateAsync<Booking>(
				CacheKeys.BookingById(request.Id),
				b => b.Id == request.Id,
				cancellationToken)
				?? throw new ApplicationException($"Booking with Id {request.Id} not found.");

			response = mapper.Map<BookingResponseDTO>(booking);
			response.IsSuccess = true;
			response.Message = "Booking retrieved successfully.";
			return response;
		}
	}
}
