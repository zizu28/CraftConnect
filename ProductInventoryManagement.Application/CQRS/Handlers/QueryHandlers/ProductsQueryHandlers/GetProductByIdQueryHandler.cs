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
			var response = new ProductResponseDTO();
			logger.LogInformation("Handling GetProductByIdQuery");
			var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
			if (product == null)
			{
				logger.LogWarning("Product with ID {ProductId} not found", request.ProductId);
				response.Message = "Product not found";
				response.IsSuccess = false;
				response.Errors = ["Product with the specified ID does not exist."];
				return response;
			}
			response = mapper.Map<ProductResponseDTO>(product);
			response.Message = "Product retrieved successfully";
			response.IsSuccess = true;
			return response;
		}
	}
}
