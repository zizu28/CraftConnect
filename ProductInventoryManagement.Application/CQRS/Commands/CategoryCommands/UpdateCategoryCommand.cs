using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands
{
	public class UpdateCategoryCommand : IRequest<CategoryResponseDTO>
	{
		public CategoryUpdateDTO CategoryUpdateDTO { get; set; }
	}
}
