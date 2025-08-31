using AutoMapper;
using Core.Logging;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.CategoriesQueryHandlers
{
	public class GetCategoryByIdQueryHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<GetCategoryByIdQueryHandler> logger,
		IMapper mapper) : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDTO>
	{
		public async Task<CategoryResponseDTO> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetCategoryByIdQuery for CategoryId: {CategoryId}", request.Id);
			var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
			if (category == null)
			{
				logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
				return new CategoryResponseDTO
				{
					IsSuccess = false,
					Message = "Category not found.",
					Errors = [$"No category found with ID {request.Id}"]
				};
			}
			var categoryDTO = mapper.Map<CategoryResponseDTO>(category);
			categoryDTO.IsSuccess = true;
			categoryDTO.Message = "Category retrieved successfully.";
			logger.LogInformation("Successfully retrieved category with ID {CategoryId}", request.Id);
			return categoryDTO;
		}
	}
}
