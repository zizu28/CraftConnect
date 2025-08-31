using Core.Logging;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.CategoryCommandHandlers
{
	public class DeleteCategoryCommandHandler(
		ICategoryRepository categoryRepository,
		ILoggingService<DeleteCategoryCommandHandler> logger) : IRequestHandler<DeleteCategoryCommand, bool>
	{
		public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
				if (category == null)
				{
					logger.LogWarning("Category with ID: {CategoryId} not found for deletion.", request.CategoryId);
					return false;
				}
				await categoryRepository.DeleteAsync(category.Id, cancellationToken);
				await categoryRepository.SaveChangesAsync(cancellationToken);
				logger.LogInformation("Category with ID: {CategoryId} deleted successfully.", request.CategoryId);
				return true;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while deleting the category with ID: {CategoryId}. Error: {Message}", request.CategoryId, ex.Message);
				return false;
			}
		}
	} 
}
