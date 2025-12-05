using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.BookingLineItemsQueries;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingLineItemsQueryHandlers
{
	public class GetAllBookingLineItemsQueryHandler(
		IBookingLineItemRepository lineItemRepository,
		IMapper mapper) : IRequestHandler<GetAllBookingLineItemsQuery, IEnumerable<BookingLineItemResponseDTO>>
	{
		public async Task<IEnumerable<BookingLineItemResponseDTO>> Handle(GetAllBookingLineItemsQuery request, CancellationToken cancellationToken)
		{
			var lineItems = await lineItemRepository.GetAllAsync(cancellationToken)
				?? throw new InvalidOperationException("No booking line items found.");
			return mapper.Map<IEnumerable<BookingLineItemResponseDTO>>(lineItems);
		}
	}
}
