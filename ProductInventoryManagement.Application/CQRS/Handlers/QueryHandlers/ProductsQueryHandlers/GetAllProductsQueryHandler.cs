using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Infrastructure.Cache;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Queries.ProductQueries;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.ProductsQueryHandlers
{
	public class GetAllProductsQueryHandler(
		IProductRepository productRepository,
		IMapper mapper,
		ILoggingService<GetAllProductsQueryHandler> logger,
		ICacheService cacheService) : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponseDTO>>
	{
		public async Task<IEnumerable<ProductResponseDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetAllProductsQuery");
			var products = await cacheService.GetOrCreateManyAsync<Product>(
				CacheKeys.AllProducts,
				p => true,
				cancellationToken);

			if (products == null || !products.Any())
			{
				logger.LogWarning("No products found");
				return [new ProductResponseDTO {
					IsSuccess = false,
					Message = "No products found",
					Errors = ["No products available in the inventory"]
				}];
			}

			var productDTOs = mapper.Map<IEnumerable<ProductResponseDTO>>(products);
			foreach (var product in productDTOs)
			{
				product.Message = "Product retrieved successfully";
				product.IsSuccess = true;
			}
			return productDTOs;
		}
	}
}
