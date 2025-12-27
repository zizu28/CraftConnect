using Core.Logging;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UserManagement.Application.CQRS.Commands.UserCommands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class ConfirmEmailCommandHandler(
		ILoggingService<ConfirmEmailCommandHandler> logger,
		ApplicationDbContext dbContext) : IRequestHandler<ConfirmEmailCommand, bool>
	{
		public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
		{
			try
			{
				// Fetch all non-expired verification tokens (can't query BCrypt hashes in SQL)
				var allTokens = await dbContext.EmailVerificationTokens
					.Include(vt => vt.User)
					.Where(vt => vt.ExpiresOnUtc > DateTime.UtcNow)
					.ToListAsync(cancellationToken);

				// Find the token that matches using BCrypt verification (plain token vs hashed token)
				var verificationRecord = allTokens
					.FirstOrDefault(vt => BCrypt.Net.BCrypt.Verify(request.token, vt.TokenValue));

				if(verificationRecord == null)
				{
					logger.LogWarning("Invalid or expired verification token attempt");
					return false;
				}

				if (verificationRecord.User.IsEmailConfirmed)
				{
					logger.LogWarning("Verification token already used for user {UserId}", verificationRecord.UserId);
					dbContext.EmailVerificationTokens.Remove(verificationRecord);
					await dbContext.SaveChangesAsync(cancellationToken);
					return true;
				}

				verificationRecord.User.IsEmailConfirmed = true;
				dbContext.EmailVerificationTokens.Remove(verificationRecord);
				await dbContext.SaveChangesAsync(cancellationToken);
				logger.LogInformation("Email verified successfully for user ID: {UserId}", verificationRecord.UserId);
				return true;
			}
			catch(Exception ex)
			{
				logger.LogError(ex, "Error occurred while confirming email for token: {Token}", request.token);
				return false;
			}
		}
	}
}
