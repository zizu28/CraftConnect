using MediatR;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries
{
	public class GetCategoryByNameQuery : IRequest<CategoryResponseDTO>
	{
		public string Name { get; set; }
	}
}
