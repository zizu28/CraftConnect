using Core.Logging;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class ConfirmEmailCommandHandler(
		ILoggingService<ConfirmEmailCommandHandler> logger,
		IUserRepository userRepository) : IRequestHandler<ConfirmEmailCommand, bool>
	{
		public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Processing email confirmation for user with email: {Email}", request.Email);
			if (string.IsNullOrEmpty(request.Email))
			{
				logger.LogWarning("Email is null or empty.");
				return false;
			}
			var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
			if (user == null)
			{
				logger.LogWarning("User with email {Email} not found.", request.Email);
				return false;
			}
			if (user.IsEmailConfirmed)
			{
				logger.LogInformation("Email for user {Email} is already confirmed.", request.Email);
				return true;
			}
			user.IsEmailConfirmed = true;
			await userRepository.UpdateAsync(user, cancellationToken);
			logger.LogInformation("Email for user {Email} confirmed successfully.", request.Email);
			return true;
		}
	}
}
