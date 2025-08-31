using MediatR;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.CQRS.Commands.ProductCommands
{
	public class CreateProductCommand : IRequest<ProductResponseDTO>
	{
		public string CraftmanEmail { get; set; }
		public ProductCreateDTO ProductDTO { get; set; }
	}
}
