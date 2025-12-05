using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Queries.ProductQueries
{
	public class GetAllProductsQuery : IRequest<IEnumerable<ProductResponseDTO>>
	{
	}
}
