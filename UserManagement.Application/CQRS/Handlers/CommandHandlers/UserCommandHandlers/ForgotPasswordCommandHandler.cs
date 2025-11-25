using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class ForgotPasswordCommandHandler(
		IBackgroundJobService backgroundJob,
		ApplicationDbContext dbContext,
		ILoggingService<ForgotPasswordCommandHandler> logger) : IRequestHandler<ForgotPasswordCommand>
	{
		public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.Address == request.Email, cancellationToken)
				?? throw new InvalidOperationException($"User with email {request.Email} not found.");
			if (!user.IsEmailConfirmed)
			{
				logger.LogWarning("User with email {Email} not verified.");
				return;
			}

			var passwordToken = await dbContext.ResetPasswordTokens
				.FirstOrDefaultAsync(rp => rp.UserId == user.Id, cancellationToken);
			if(passwordToken != null)
			{
				dbContext.ResetPasswordTokens.Remove(passwordToken);
			}

			var resetPasswordValue = ResetPasswordToken.GenerateToken();
			var hashedToken = BCrypt.Net.BCrypt.HashPassword(resetPasswordValue);
			var resetPasswordToken = new ResetPasswordToken
			{
				ResetPasswordTokenId = Guid.NewGuid(),
				TokenValue = hashedToken,
				User = user,
				UserId = user.Id,
				CreatedOnUtc = DateTime.UtcNow,
				ExpiresOnUtc = DateTime.UtcNow.AddHours(1)
			};

			await dbContext.ResetPasswordTokens.AddAsync(resetPasswordToken, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			string encodedToken = WebUtility.UrlEncode(resetPasswordValue);
			string link = $"https://localhost:7284/reset-password?email={request.Email}&token={encodedToken}";
			backgroundJob.Enqueue<IGmailService>(
				"forgot-password",
				password => password.SendEmailAsync(
					request.Email,
					"Forgotten Password",
					$"<p>You requested a password reset.</p>" +
					$"<p>Click <a href='{link}'>here</a> to reset your password.</p>" +
					$"<p>This link expires in 1 hour.</p>",
					true,
					CancellationToken.None));
			logger.LogInformation($"Password reset email sent to {request.Email}");
		}
	}
}
