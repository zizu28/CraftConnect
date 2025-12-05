using Core.SharedKernel.DTOs;
using MediatR;

namespace ProductInventoryManagement.Application.CQRS.Commands.ProductCommands
{
	public class UpdateProductCommand : IRequest<ProductResponseDTO>
	{
		public ProductUpdateDTO ProductDTO { get; set; }
	}
}
