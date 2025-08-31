using MediatR;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries
{
	public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryResponseDTO>>
	{
	}
}
