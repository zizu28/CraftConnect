using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class ChangePasswordCommandHandler(
		IBackgroundJobService background,
		ILogger<ChangePasswordCommandHandler> logger,
		IUserRepository repository, 
		IHttpContextAccessor contextAccessor) 
		: IRequestHandler<ChangePasswordCommand, Unit>
	{

		public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
		{
			if (!contextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
			{
				logger.LogError("User with email {Username} not authenticated.", request.Email);
				throw new InvalidOperationException($"User with email {request.Email} not found.");
			}
			var user = await repository.FindBy(u => u.Email.Address == request.Email, cancellationToken);
			if (user == null)
			{
				logger.LogWarning("User with username with email {Username} not found.", request.Email);
				throw new KeyNotFoundException($"User with email {request.Email} not found.");
			}
			if(!user.IsEmailConfirmed)
			{
				logger.LogWarning("User with email {Username} has not confirmed their email.", request.Email);
				throw new InvalidOperationException("User has not confirmed their email.");
			}
			if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
			{
				logger.LogWarning("Old password for user with email {Username} is incorrect.", request.Email);
				throw new UnauthorizedAccessException("Old password is incorrect.");
			}
			logger.LogInformation("Changing password for user with email {Username}.", request.Email);
			var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, salt);
			await repository.UpdateAsync(user, cancellationToken);
			background.Enqueue<IEmailService>(
				"Change-password",
				email => email.SendEmailAsync(user.Email.Address, "Password Change", "", true, CancellationToken.None));
			logger.LogInformation("Password changed successfully for user with email {Username}.", request.Email);
			return Unit.Value;
		}
	}
}
