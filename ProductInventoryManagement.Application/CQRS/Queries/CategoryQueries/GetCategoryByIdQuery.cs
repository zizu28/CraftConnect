using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries
{
	public class GetCategoryByIdQuery : IRequest<CategoryResponseDTO>
	{
		public Guid Id { get; set; }
	}
}
