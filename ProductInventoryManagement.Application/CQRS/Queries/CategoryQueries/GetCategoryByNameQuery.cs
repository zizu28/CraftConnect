using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries
{
	public class GetCategoryByNameQuery : IRequest<CategoryResponseDTO>
	{
		public string Name { get; set; }
	}
}
