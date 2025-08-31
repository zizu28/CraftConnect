using MediatR;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands
{
	public class CategoryCreateCommand : IRequest<CategoryResponseDTO>
	{
		public CategoryCreateDTO CategoryCreateDTO { get; set; }
	}
}
