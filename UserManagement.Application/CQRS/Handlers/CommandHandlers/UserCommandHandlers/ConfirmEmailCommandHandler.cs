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
				var verificationRecord = await dbContext.EmailVerificationTokens
					.Include(vt => vt.User)
					.FirstOrDefaultAsync(vt => vt.TokenValue == request.token, cancellationToken);
				if(verificationRecord == null)
				{
					logger.LogWarning("Invalid or expired verification token used: {TokenHash}", request.token);
					throw new Exception($"Invalid or expired verification token used: {request.token}");
				}

				if (verificationRecord.User.IsEmailConfirmed)
				{
					logger.LogWarning("Verification token already used: {TokenHash}", request.token);
					dbContext.EmailVerificationTokens.Remove(verificationRecord);
					await dbContext.SaveChangesAsync(cancellationToken);
					return true;
				}

				if (verificationRecord.ExpiresOnUtc < DateTime.UtcNow)
				{
					logger.LogWarning("Expired verification token used: {TokenHash}", request.token);
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
