using Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Validators.UserValidators;
using UserManagement.Domain.Entities;

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
			RefreshToken? refreshToken = await dbContext.RefreshTokens
				.Include(rt => rt.User)
				.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);
			if (refreshToken == null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
			{
				throw new ApplicationException("Invalid or expired refresh token.");
			}
			if (refreshToken.User == null)
			{
				throw new ApplicationException("User associated with the refresh token not found.");
			}
			if (!refreshToken.User.IsEmailConfirmed)
			{
				throw new ApplicationException("User's email is not confirmed.");
			}
			if (refreshToken.User.RefreshTokens.Count >= 5)
			{
				throw new ApplicationException("User has reached the maximum number of refresh tokens.");
			}
			// Remove old refresh tokens
			var oldTokens = refreshToken.User.RefreshTokens
				.Where(rt => rt.Token != request.RefreshToken && rt.ExpiresOnUtc < DateTime.UtcNow)
				.ToList();
			if (oldTokens.Count != 0)
			{
				dbContext.RefreshTokens.RemoveRange(oldTokens);
				await dbContext.SaveChangesAsync(cancellationToken);
			}
			// Generate new tokens
			var newAccessToken = tokenProvider.GenerateAccessToken(refreshToken.User.Id, refreshToken.User.Email.Address, refreshToken.User.Role.ToString());
			var newRefreshToken = await tokenProvider.GenerateRefreshToken(refreshToken.User);
			// Update the refresh token in the database
			refreshToken.Token = newRefreshToken;
			refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(7);
			dbContext.RefreshTokens.Update(refreshToken);
			await dbContext.SaveChangesAsync(cancellationToken);
			return (newAccessToken, newRefreshToken);
		}
	}
}
