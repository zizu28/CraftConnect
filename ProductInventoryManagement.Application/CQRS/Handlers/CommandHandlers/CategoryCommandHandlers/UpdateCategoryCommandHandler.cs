using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;
using ProductInventoryManagement.Application.Validators.CategoryValidators;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.CategoryCommandHandlers
{
	public class UpdateCategoryCommandHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<UpdateCategoryCommandHandler> logger,
		IMapper mapper,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, CategoryResponseDTO>
	{
		public async Task<CategoryResponseDTO> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			var response = new CategoryResponseDTO();
			var validator = new CategoryUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.CategoryUpdateDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Category update failed due to validation errors.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogWarning("Category update failed due to validation errors: {Errors}", response.Errors);
				return response;
			}
			await unitOfWork.BeginTransactionAsync(cancellationToken);
			try
			{
				var category = await categoryRepository.GetByIdAsync(request.CategoryUpdateDTO.CategoryId, cancellationToken);
				if (category == null)
				{
					response.IsSuccess = false;
					response.Message = "Category not found.";
					logger.LogWarning("Category with ID: {CategoryId} not found for update.", request.CategoryUpdateDTO.CategoryId);
					return response;
				}

				mapper.Map(request.CategoryUpdateDTO, category);
				await categoryRepository.UpdateAsync(category, cancellationToken);
				await unitOfWork.SaveChangesAsync(cancellationToken);
				response = mapper.Map<CategoryResponseDTO>(category);
				response.IsSuccess = true;
				response.Message = "Category updated successfully.";
				logger.LogInformation("Category with ID: {CategoryId} updated successfully.", response.CategoryId);
				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = "An error occurred while updating the category.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogError(ex, "An error occurred while updating the category: {Message}", ex.Message);
				await unitOfWork.RollbackTransactionAsync(cancellationToken);
				return response;
			}
		}
	}
}
