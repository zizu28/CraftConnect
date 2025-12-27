using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	/// <summary>
	/// Handler for changing password while user is authenticated (requires old password verification)
	/// </summary>
	public class ChangePasswordWhileAuthenticatedCommandHandler(
		IBackgroundJobService background,
		ILogger<ChangePasswordWhileAuthenticatedCommandHandler> logger,
		ApplicationDbContext dbContext) 
		: IRequestHandler<ChangePasswordWhileAuthenticatedCommand, Unit>
	{
		public async Task<Unit> Handle(ChangePasswordWhileAuthenticatedCommand request, CancellationToken cancellationToken)
		{
			var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
			if (user == null)
			{
				logger.LogWarning("Password change attempt for non-existent user {UserId}", request.UserId);
				throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
			}

			if(!user.IsEmailConfirmed)
			{
				logger.LogWarning("Password change attempt for unverified user {UserId}", request.UserId);
				throw new InvalidOperationException("Email must be confirmed before changing password.");
			}

			// Verify old password
			var isOldPasswordValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
			if (!isOldPasswordValid)
			{
				logger.LogWarning("Invalid old password provided for user {UserId}", request.UserId);
				throw new UnauthorizedAccessException("Current password is incorrect.");
			}

			// Validate new password matches confirmation
			if (!request.NewPassword.Equals(request.ConfirmPassword))
			{
				logger.LogWarning("New password and confirmation do not match for user {UserId}", request.UserId);
				throw new ArgumentException("New password and confirmation password do not match.");
			}

			// Ensure new password is different from old password
			if (request.OldPassword.Equals(request.NewPassword))
			{
				logger.LogWarning("User {UserId} attempted to change to same password", request.UserId);
				throw new ArgumentException("New password must be different from current password.");
			}

			logger.LogInformation("Changing password for authenticated user {UserId}", request.UserId);
			var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, salt);
			
			dbContext.Users.Update(user);
			await dbContext.SaveChangesAsync(cancellationToken);

			// Send notification email
			background.Enqueue<IGmailService>(
				"default",
				email => email.SendEmailAsync(
					user.Email.Address, 
					"Password Changed Successfully", 
					"<p>Your password has been changed successfully.</p>" +
					"<p>If you did not make this change, please contact support immediately.</p>", 
					true, 
					CancellationToken.None));

			logger.LogInformation("Password changed successfully for user {UserId}", request.UserId);
			return Unit.Value;
		}
	}
}
