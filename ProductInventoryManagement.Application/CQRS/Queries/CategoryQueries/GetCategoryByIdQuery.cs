using MediatR;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries
{
	public class GetCategoryByIdQuery : IRequest<CategoryResponseDTO>
	{
		public Guid Id { get; set; }
	}
}
