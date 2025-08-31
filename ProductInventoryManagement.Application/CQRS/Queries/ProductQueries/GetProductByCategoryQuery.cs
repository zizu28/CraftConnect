using MediatR;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.CQRS.Queries.ProductQueries
{
	public class GetProductByCategoryQuery : IRequest<IEnumerable<ProductResponseDTO>>
	{
		public string Category { get; set; }
	}
}
