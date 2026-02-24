using Infrastructure.Cache;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CraftmanCommandHandlers
{
	public class DeleteCraftmanCommandHandler(
		ICraftsmanRepository craftmanRepository,
		ILogger<DeleteCraftmanCommandHandler> logger,
		IUnitOfWork unitOfWork,
		ICacheService cacheService) : IRequestHandler<DeleteCraftmanCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteCraftmanCommand request, CancellationToken cancellationToken)
		{
			return await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				var craftman = await craftmanRepository.GetByIdAsync(request.CraftmanId, cancellationToken)
					?? throw new KeyNotFoundException($"Craftman with ID {request.CraftmanId} not found.");
				
				await craftmanRepository.DeleteAsync(craftman.Id, cancellationToken);
				logger.LogInformation("Craftman with ID {CraftmanId} deleted successfully.", craftman.Id);

				// Evict both the per-craftsman key and the full list.
				await cacheService.RemoveSync(CacheKeys.CraftsmanById(craftman.Id), cancellationToken);
				await cacheService.RemoveSync(CacheKeys.AllCraftsmen, cancellationToken);

				return Unit.Value;
			}, cancellationToken);
		}
	}
}
