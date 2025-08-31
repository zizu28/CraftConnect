using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands
{
	public class DeleteCategoryCommand : IRequest<bool>
	{
		public int CategoryId { get; set; }
	}
}
