using MediatR;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.CQRS.Queries.ProductQueries
{
	public class GetProductByIdQuery : IRequest<ProductResponseDTO>
	{
		public Guid ProductId { get; set; }
	}
}
