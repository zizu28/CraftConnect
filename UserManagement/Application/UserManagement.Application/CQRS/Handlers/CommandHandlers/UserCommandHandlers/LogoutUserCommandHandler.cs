using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class LogoutUserCommandHandler(
		IUserRepository user,
		ILogger<LogoutUserCommandHandler> logger) : IRequestHandler<LogoutUserCommand, Unit>
	{
		private readonly IRepository<User> user = user;
		private readonly ILogger<LogoutUserCommandHandler> logger = logger;

		public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling LogoutUserCommand for user: {Username}", request.Username);
			var userEntity = await user.FindBy(u => u.Username == request.Username, cancellationToken);
			if (userEntity == null)
			{
				logger.LogWarning("User not found: {Username}", request.Username);
				return Unit.Value;
			}
			logger.LogInformation("User {Username} logged out successfully.", request.Username);
			return Unit.Value;
		}
	}
}
