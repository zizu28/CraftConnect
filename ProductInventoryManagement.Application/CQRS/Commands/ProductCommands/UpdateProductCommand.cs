using MediatR;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.CQRS.Commands.ProductCommands
{
	public class UpdateProductCommand : IRequest<ProductResponseDTO>
	{
		public ProductUpdateDTO ProductDTO { get; set; }
	}
}
