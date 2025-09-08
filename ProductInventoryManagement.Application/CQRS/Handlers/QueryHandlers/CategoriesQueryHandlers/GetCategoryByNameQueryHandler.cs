using AutoMapper;
using Core.Logging;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;

namespace ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.CategoriesQueryHandlers
{
	public class GetCategoryByNameQueryHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<GetCategoryByNameQueryHandler> logger,
		IMapper mapper) : IRequestHandler<GetCategoryByNameQuery, CategoryResponseDTO>
	{
		public async Task<CategoryResponseDTO> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var category = await categoryRepository.GetCategoryByNameAsync(request.Name, cancellationToken);
				if (category == null)
				{
					logger.LogWarning($"Category with name {request.Name} not found.");
					throw new KeyNotFoundException($"Category with name {request.Name} not found.");
				}
				var categoryDto = mapper.Map<CategoryResponseDTO>(category);
				categoryDto.IsSuccess = true;
				categoryDto.Message = "Category retrieved successfully.";
				return categoryDto;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"An error occurred while retrieving category with name {request.Name}.");
				throw;
			}
		}
	}
}
