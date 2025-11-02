using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Validators.UserValidators;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class ResendEmailCommandHandler(
		IBackgroundJobService backgroundJob,
		ApplicationDbContext dbContext,
		ILoggingService<ResendEmailCommandHandler> logger) : IRequestHandler<ResendEmailCommand, Unit>
	{
		public async Task<Unit> Handle(ResendEmailCommand request, CancellationToken cancellationToken)
		{
			var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.Address
				.Equals(request.Email), cancellationToken)
				?? throw new Exception($"User with email {request.Email} not yet registered.");

			var validator = new ResendEmailCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogError(new Exception(), "Provide a valid email");
				throw new Exception("Invalid email.");
			}
			
			var verificationToken = await dbContext.EmailVerificationTokens
				.FirstOrDefaultAsync(vt => vt.UserId == user.Id, cancellationToken);
			if(verificationToken != null)
			{
				dbContext.EmailVerificationTokens.Remove(verificationToken);
				await dbContext.SaveChangesAsync(cancellationToken);
			}			

			var verificationTokenValue = EmailVerificationToken.GenerateToken();
			var hashedToken = BCrypt.Net.BCrypt.HashPassword(verificationTokenValue);

			var emailVerificationToken = new EmailVerificationToken
			{
				EmailVerificationTokenId = Guid.NewGuid(),
				TokenValue = hashedToken,
				User = user,
				UserId = user.Id,
				CreatedOnUtc = DateTime.UtcNow,
				ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
			};
			await dbContext.EmailVerificationTokens.AddAsync(emailVerificationToken, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			string? verificationLink = $"https://localhost:7272/users/confirm-email?token={hashedToken}";
			backgroundJob.Enqueue<IGmailService>(
				"email-resend",
				resend => resend.SendEmailAsync(
					request.Email,
					"Email Verification from CraftConnect",
					$"A welcome message. To verify your email address, <a href='{verificationLink}'>click here</a>.",
					true,
					CancellationToken.None
				)
			);
			return Unit.Value;
		}
	}
}
