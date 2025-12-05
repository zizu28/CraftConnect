using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Queries.ProductQueries;

namespace ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.ProductsQueryHandlers
{
	public class GetProductByCategoryQueryHandler(
		IProductRepository productRepository,
		IMapper mapper,
		ILoggingService<GetProductByCategoryQueryHandler> logger) : IRequestHandler<GetProductByCategoryQuery, IEnumerable<ProductResponseDTO>>
	{
		public async Task<IEnumerable<ProductResponseDTO>> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling SearchProductsQuery");
			var products = await productRepository.SearchProductsAsync(request.Category, cancellationToken);
			if(products == null || !products.Any())
			{
				logger.LogWarning("No products found matching the search term: {SearchTerm}", request.Category);
				return [new ProductResponseDTO {
					IsSuccess = false,
					Message = "No products found matching the search term.",
					Errors = ["No products found matching the search term."]
				}];
			}
			var productDTOs = mapper.Map<IEnumerable<ProductResponseDTO>>(products);
			foreach(var product in productDTOs)
			{
				product.Message = "Product retrieved successfully";
				product.IsSuccess = true;
			}
			return productDTOs;
		}
	}
}
