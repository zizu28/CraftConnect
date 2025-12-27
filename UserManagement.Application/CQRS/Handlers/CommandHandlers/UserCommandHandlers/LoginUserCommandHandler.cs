using Core.Logging;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Responses;
using UserManagement.Application.Validators.UserValidators;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class LoginUserCommandHandler(
		ILoggingService<LoginUserCommandHandler> _logger,
		ITokenProvider _refreshToken,
		IUnitOfWork unitOfWork,
		ApplicationDbContext dBContext) 
		: IRequestHandler<LoginUserCommand, LoginResponse>
	{

		public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
		{
			var response = new LoginResponse();
			var validator = new LoginUserCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validation failed for login request");
				return response;
			}

			// Fetch user - don't throw exception to prevent email enumeration
			var user = await dBContext.Users
				.Include(u => u.RefreshTokens)
				.FirstOrDefaultAsync(u => u.Email.Address == request.Email, cancellationToken);

			// Always perform BCrypt verification to prevent timing attacks
			// Use a dummy hash if user doesn't exist
			var hashToVerify = user?.PasswordHash ?? "$2a$12$dummyhashtopreventtimingattacksonemailenumeration";
			var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, hashToVerify);

			// If user doesn't exist or password is invalid, return generic error
			if (user == null || !isPasswordValid)
			{
				_logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
				return response; // Empty response - generic error to frontend
			}

			// Check email confirmation
			if (!user.IsEmailConfirmed)
			{
				_logger.LogWarning("Login attempt with unconfirmed email: {Email}", request.Email);
				return response; // Same generic error
			}

			// Revoke all existing refresh tokens for this user (security measure)
			var refreshTokens = user.RefreshTokens;
			if (refreshTokens != null)
			{
				foreach (var refreshToken in refreshTokens)
				{
					refreshToken.IsRevoked = true;
					refreshToken.RevokedOnUtc = DateTime.UtcNow;
					refreshToken.RevokedReason = "Replaced";
				}
			}

			// Successful login - generate new tokens
			response.UserId = user.Id;
			response.AccessToken = _refreshToken.GenerateAccessToken(user.Id, user.Email.Address, user.Role.ToString());
			response.RefreshToken = _refreshToken.GenerateRefreshToken(user);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("User with email {Email} logged in successfully.", request.Email);

			return response;
		}
	}
}
