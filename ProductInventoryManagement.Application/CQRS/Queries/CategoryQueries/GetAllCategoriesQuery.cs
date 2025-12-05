using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries
{
	public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryResponseDTO>>
	{
	}
}
