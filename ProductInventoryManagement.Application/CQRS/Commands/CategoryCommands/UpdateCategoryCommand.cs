using MediatR;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands
{
	public class UpdateCategoryCommand : IRequest<CategoryResponseDTO>
	{
		public CategoryUpdateDTO CategoryUpdateDTO { get; set; }
	}
}
