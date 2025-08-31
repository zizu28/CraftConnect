using AutoMapper;
using Core.Logging;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Queries.ProductQueries;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;

namespace ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.ProductsQueryHandlers
{
	public class GetProductByIdQueryHandler(
		IProductRepository productRepository,
		IMapper mapper,
		ILoggingService<GetProductByIdQueryHandler> logger) : IRequestHandler<GetProductByIdQuery, ProductResponseDTO>
	{
		public async Task<ProductResponseDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetProductByIdQuery");
			var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
			if (product == null)
			{
				logger.LogWarning("Product with ID {ProductId} not found", request.ProductId);
				return new ProductResponseDTO
				{
					IsSuccess = false,
					Message = "Product not found",
					Errors = ["Product with the specified ID does not exist."]
				};
			}
			var productDTO = mapper.Map<ProductResponseDTO>(product);
			productDTO.Message = "Product retrieved successfully";
			productDTO.IsSuccess = true;
			return productDTO;
		}
	}
}
