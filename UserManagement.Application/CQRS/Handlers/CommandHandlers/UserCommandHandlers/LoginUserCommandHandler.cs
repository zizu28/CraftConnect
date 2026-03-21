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

			var user = await dBContext.Users
				.Include(u => u.RefreshTokens)
				.FirstOrDefaultAsync(u => u.Email.Address == request.Email, cancellationToken);

			// Always perform BCrypt verification to prevent timing attacks
			// Use a dummy hash if user doesn't exist
			var hashToVerify = user?.PasswordHash ?? "$2a$12$dummyhashtopreventtimingattacksonemailenumeration";
			var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, hashToVerify);

			if (user == null || !isPasswordValid)
			{
				_logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
				return response;
			}

			if (!user.IsEmailConfirmed)
			{
				_logger.LogWarning("Login attempt with unconfirmed email: {Email}", request.Email);
				return response;
			}

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

			response.UserId = user.Id;
			response.AccessToken = _refreshToken.GenerateAccessToken(user.Id, user.Email.Address, user.Role.ToString());
			response.RefreshToken = _refreshToken.GenerateRefreshToken(user);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("User with email {Email} logged in successfully.", request.Email);

			return response;
		}
	}
}
