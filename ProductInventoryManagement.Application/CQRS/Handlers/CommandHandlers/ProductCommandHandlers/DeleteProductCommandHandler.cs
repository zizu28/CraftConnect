using Core.Logging;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.ProductCommandHandlers
{
	public class DeleteProductCommandHandler(
		IProductRepository productRepository,
		ILoggingService<DeleteProductCommandHandler> logger) : IRequestHandler<DeleteProductCommand, bool>
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
				await productRepository.DeleteAsync(existingProduct.Id, cancellationToken);
				await productRepository.SaveChangesAsync(cancellationToken);
				logger.LogInformation("Product deleted successfully with ID: {ProductId}", existingProduct.Id);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while deleting the product: {ErrorMessage}", ex.Message);
			}
			return true;
		}
	}
}
