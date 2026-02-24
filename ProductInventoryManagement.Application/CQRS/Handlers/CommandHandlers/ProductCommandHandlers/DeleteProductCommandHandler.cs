using Core.Logging;
using Infrastructure.Cache;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.ProductCommandHandlers
{
	public class DeleteProductCommandHandler(
		IProductRepository productRepository,
		ILoggingService<DeleteProductCommandHandler> logger,
		IUnitOfWork unitOfWork,
		ICacheService cacheService) : IRequestHandler<DeleteProductCommand, bool>
	{
		public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var existingProduct = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
				if (existingProduct == null)
				{
					logger.LogWarning("Product with ID: {ProductId} not found for deletion.", request.ProductId);
					return false;
				}
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await productRepository.DeleteAsync(existingProduct.Id, cancellationToken);
				}, cancellationToken);
				logger.LogInformation("Product deleted successfully with ID: {ProductId}", existingProduct.Id);

				// Evict both the per-product key and the all-products list.
				await cacheService.RemoveSync(CacheKeys.ProductById(existingProduct.Id), cancellationToken);
				await cacheService.RemoveSync(CacheKeys.AllProducts, cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while deleting the product: {ErrorMessage}", ex.Message);
			}
			return true;
		}
	}
}
