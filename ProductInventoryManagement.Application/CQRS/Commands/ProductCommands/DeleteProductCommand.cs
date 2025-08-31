using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Commands.ProductCommands
{
	public class DeleteProductCommand : IRequest<bool>
	{
		public Guid ProductId { get; set; }
	}
}
