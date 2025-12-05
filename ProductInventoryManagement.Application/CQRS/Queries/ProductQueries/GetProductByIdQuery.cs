using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Queries.ProductQueries
{
	public class GetProductByIdQuery : IRequest<ProductResponseDTO>
	{
		public Guid ProductId { get; set; }
	}
}
