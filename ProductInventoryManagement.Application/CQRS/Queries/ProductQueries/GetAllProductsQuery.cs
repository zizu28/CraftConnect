using MediatR;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.CQRS.Queries.ProductQueries
{
	public class GetAllProductsQuery : IRequest<IEnumerable<ProductResponseDTO>>
	{
	}
}
