using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Validators.UserValidators;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class RefreshTokenCommandHandler(
		ApplicationDbContext dbContext,
		ITokenProvider tokenProvider) : IRequestHandler<RefreshTokenCommand, (string AccessToken, string RefreshToken)>
	{
		public async Task<(string AccessToken, string RefreshToken)> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
		{
			var validator = new RefreshTokenCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				throw new ApplicationException("Invalid refresh token request.");
			}
			var user = await dbContext.Users
				.Include(u => u.RefreshTokens)
				.FirstOrDefaultAsync(u => u.RefreshTokens
					.Any(rt => rt.Token == request.RefreshToken), cancellationToken) 
				?? throw new ApplicationException("Invalid token.");
			var existingToken = user.RefreshTokens.First(rt => rt.Token == request.RefreshToken);
		
			// TOKEN REUSE DETECTION: Security measure to detect token theft
			// If a revoked/expired token is used, it indicates potential compromise
			// We revoke ALL user's tokens to protect the account
			if (existingToken.ExpiresOnUtc < DateTime.UtcNow || existingToken.IsRevoked)
			{
				// Revoke all tokens - force re-login on all devices
				foreach (var t in user.RefreshTokens) { t.IsRevoked = true; }
				await dbContext.SaveChangesAsync(cancellationToken);
				// TODO: Consider sending security alert email to user
				throw new ApplicationException("Invalid or expired token.");
			}
			existingToken.IsRevoked = true;
			existingToken.RevokedOnUtc = DateTime.UtcNow;
			existingToken.RevokedReason = "Rotated";
			var newRefreshTokenString = tokenProvider.GenerateRefreshToken(user);
			var newAccessToken = tokenProvider.GenerateAccessToken(user.Id, user.Email.Address, user.Role.ToString());
			await dbContext.SaveChangesAsync(cancellationToken);
			return (newAccessToken, newRefreshTokenString);
		}
	}
}
