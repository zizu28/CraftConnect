using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Queries.ProductQueries
{
	public class GetProductByCategoryQuery : IRequest<IEnumerable<ProductResponseDTO>>
	{
		public string Category { get; set; }
	}
}
