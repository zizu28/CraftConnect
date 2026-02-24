using Core.Logging;
using Infrastructure.Cache;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.CategoryCommandHandlers
{
	public class DeleteCategoryCommandHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<DeleteCategoryCommandHandler> logger,
		IUnitOfWork unitOfWork,
		ICacheService cacheService) : IRequestHandler<DeleteCategoryCommand, bool>
	{
		public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			try
			{
				await unitOfWork.BeginTransactionAsync(cancellationToken);
				var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
				if (category == null)
				{
					logger.LogWarning("Category with ID: {CategoryId} not found for deletion.", request.CategoryId);
					return false;
				}
				await categoryRepository.DeleteAsync(category.Id, cancellationToken);
				await unitOfWork.SaveChangesAsync(cancellationToken);
				logger.LogInformation("Category with ID: {CategoryId} deleted successfully.", request.CategoryId);

				// Evict both the list and the per-category entry.
				await cacheService.RemoveSync(CacheKeys.CategoryById(request.CategoryId), cancellationToken);
				await cacheService.RemoveSync(CacheKeys.AllCategories, cancellationToken);

				return true;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while deleting the category with ID: {CategoryId}. Error: {Message}", request.CategoryId, ex.Message);
				await unitOfWork.RollbackTransactionAsync(cancellationToken);
				return false;
			}
		}
	} 
}
