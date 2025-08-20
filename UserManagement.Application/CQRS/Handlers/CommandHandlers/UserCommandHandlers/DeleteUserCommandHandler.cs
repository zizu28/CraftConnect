using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class DeleteUserCommandHandler(
		IUserRepository user,
		ILogger<DeleteUserCommandHandler> logger) : IRequestHandler<DeleteUserCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling DeleteUserCommand for user with Id: {UserId}", request.UserId);
			var userEntity = await user.FindBy(u => u.Id == request.UserId, cancellationToken);
			if (userEntity == null)
			{
				logger.LogWarning("User with user id {Username} not found.", request.UserId);
				throw new KeyNotFoundException($"User with user id {request.UserId} not found.");
			}
			await user.DeleteAsync(userEntity.Id, cancellationToken);
			logger.LogInformation("User {Username} deleted successfully.", request.UserId);
			return Unit.Value;
		}
	}
}
