using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands
{
	public class CategoryCreateCommand : IRequest<CategoryResponseDTO>
	{
		public CategoryCreateDTO CategoryCreateDTO { get; set; }
	}
}
