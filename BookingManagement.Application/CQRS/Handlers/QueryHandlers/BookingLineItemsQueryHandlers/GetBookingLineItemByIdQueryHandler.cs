using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingLineItemsQueries;
using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingLineItemsQueryHandlers
{
	public class GetBookingLineItemByIdQueryHandler(
		IBookingLineItemRepository lineItemRepository,
		IMapper mapper) : IRequestHandler<GetBookingLineItemByIdQuery, BookingLineItemResponseDTO>
	{
		public async Task<BookingLineItemResponseDTO> Handle(GetBookingLineItemByIdQuery request, CancellationToken cancellationToken)
		{
			var lineItem = await lineItemRepository.GetByIdAsync(request.Id, cancellationToken)
				?? throw new KeyNotFoundException($"Booking line item with ID {request.Id} not found.");
			return mapper.Map<BookingLineItemResponseDTO>(lineItem);
		}
	}
}
