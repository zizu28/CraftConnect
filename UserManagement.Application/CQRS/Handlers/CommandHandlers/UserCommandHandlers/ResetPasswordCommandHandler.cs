using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class ResetPasswordCommandHandler(
		IBackgroundJobService background,
		ILogger<ResetPasswordCommandHandler> logger,
		ApplicationDbContext dbContext) 
		: IRequestHandler<ResetPasswordCommand, Unit>
	{
		public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.Address == request.Email, cancellationToken);
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
			// Fetch all non-expired reset tokens for this user (can't query BCrypt hashes in SQL)
			var allTokens = await dbContext.ResetPasswordTokens
				.Include(rt => rt.User)
				.Where(rt => rt.UserId == user.Id && rt.ExpiresOnUtc > DateTime.UtcNow)
				.ToListAsync(cancellationToken);

			// Find the token that matches using BCrypt verification (plain token vs hashed token)
			var resetToken = allTokens
				.FirstOrDefault(rt => BCrypt.Net.BCrypt.Verify(request.Token, rt.TokenValue));

			if(resetToken == null)
			{
				logger.LogWarning("Invalid or expired reset token for user {Email}", request.Email);
				throw new UnauthorizedAccessException("Invalid or expired password reset token.");
			}

			// Remove the used token
			dbContext.ResetPasswordTokens.Remove(resetToken);

			logger.LogInformation("Changing password for user with email {Username}.", request.Email);
			var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
			if (request.NewPassword.Equals(request.ConfirmPassword))
			{
				user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, salt);
			}
			
			dbContext.Users.Update(user);
			await dbContext.SaveChangesAsync(cancellationToken);
			background.Enqueue<IGmailService>(
				"default",
				email => email.SendEmailAsync(
					user.Email.Address, 
					"Password Changed", 
					"Password successfully changed as requested.", 
					true, 
					CancellationToken.None));
			logger.LogInformation("Password changed successfully for user with email {Username}.", request.Email);
			return Unit.Value;
		}
	}
}
