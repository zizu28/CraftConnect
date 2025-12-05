using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries;

namespace ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.CategoriesQueryHandlers
{
	public class GetAllCategoriesQueryHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<GetAllCategoriesQueryHandler> logger,
		IMapper mapper) : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryResponseDTO>>
	{
		public async Task<IEnumerable<CategoryResponseDTO>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetAllCategoriesQuery");
			var categories = await categoryRepository.GetAllAsync(cancellationToken);
			if (categories == null || !categories.Any())
			{
				logger.LogWarning("No categories found");
				return [new CategoryResponseDTO
					{
						IsSuccess = false,
						Message = "No categories found.",
						Errors = ["The category list is empty."]
					}
				];
			}
			var categoryDTOs = mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
			logger.LogInformation("Successfully retrieved {Count} categories", categoryDTOs.Count());
			foreach (var dto in categoryDTOs)
			{
				dto.IsSuccess = true;
				dto.Message = "Categories retrieved successfully.";
			}
			return categoryDTOs;
		}
	}
}
