using AutoMapper;
using Core.Logging;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;
using ProductInventoryManagement.Application.DTOs.CategoryDTOs;
using ProductInventoryManagement.Application.Validators.CategoryValidators;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.CategoryCommandHandlers
{
	public class CategoryCreateCommandHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<CategoryCreateCommandHandler> logger,
		IMapper mapper) : IRequestHandler<CategoryCreateCommand, CategoryResponseDTO>
	{
		public async Task<CategoryResponseDTO> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
		{
			var response = new CategoryResponseDTO();
			var validator = new CategoryCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.CategoryCreateDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Category creation failed due to validation errors.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogWarning("Category creation failed due to validation errors: {Errors}", response.Errors);
				return response;
			}
			try
			{
				var categoryEntity = mapper.Map<Category>(request.CategoryCreateDTO);
				await categoryRepository.AddAsync(categoryEntity, cancellationToken);
				await categoryRepository.SaveChangesAsync(cancellationToken);
				response = mapper.Map<CategoryResponseDTO>(categoryEntity);
				response.IsSuccess = true;
				response.Message = "Category created successfully.";
				logger.LogInformation("Category created successfully with ID: {CategoryId}", response.CategoryId);
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = "An error occurred while creating the category.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogError(ex, "An error occurred while creating the category: {Message}", ex.Message);
				return response;
			}
		}
	}
}
