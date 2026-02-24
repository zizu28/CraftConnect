using Core.Logging;
using Infrastructure.Cache;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class DeleteUserCommandHandler(
		ApplicationDbContext dbContext,
		ILoggingService<DeleteUserCommandHandler> logger, 
		IUnitOfWork unitOfWork,
		ICacheService cacheService) : IRequestHandler<DeleteUserCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling DeleteUserCommand for user with Id: {UserId}", request.UserId);
			var userEntity = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
			if (userEntity == null)
			{
				logger.LogWarning("User with user id {Username} not found.", request.UserId);
				throw new KeyNotFoundException($"User with user id {request.UserId} not found.");
			}
			var resetTokens = await dbContext.ResetPasswordTokens
				.Where(x => x.UserId == userEntity.Id)
				.ToListAsync(cancellationToken);
			if (resetTokens.Count > 0)
			{
				dbContext.ResetPasswordTokens.RemoveRange(resetTokens);
			}

			var emailTokens = await dbContext.EmailVerificationTokens
				.Where(x => x.UserId == userEntity.Id)
				.ToListAsync(cancellationToken);

			if (emailTokens.Count > 0)
			{
				dbContext.EmailVerificationTokens.RemoveRange(emailTokens);
			}

			dbContext.Users.Remove(userEntity);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			logger.LogInformation("User {Username} deleted successfully.", request.UserId);

			// Evict both the per-user key and the all-users list.
			await cacheService.RemoveSync(CacheKeys.UserById(request.UserId), cancellationToken);
			await cacheService.RemoveSync(CacheKeys.AllUsers, cancellationToken);

			return Unit.Value;
		}
	}
}
