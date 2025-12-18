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
				.FirstOrDefaultAsync(u => u.Email.Address == request.Email, cancellationToken)
				?? throw new KeyNotFoundException($"User with email {request.Email} not found.");
			var refreshTokens = user.RefreshTokens;
			if (refreshTokens != null)
			{
				foreach ( var refreshToken in refreshTokens )
				{
					refreshToken.IsRevoked = true;
					refreshToken.RevokedOnUtc = DateTime.UtcNow;
					refreshToken.RevokedReason = "Replaced";
				}
			}
			var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
			if(!isPasswordValid)
			{
				_logger.LogWarning("Invalid login attempt for user with email: {Email}", request.Email);
				return response;
			}
			bool isEmailConfirmed = user.IsEmailConfirmed;
			if (!isEmailConfirmed)
			{
				_logger.LogWarning("Invalid login attempt for user with email: {Email}", request.Email);
				return response;
			}
			response.UserId = user.Id;
			response.AccessToken = _refreshToken.GenerateAccessToken(user.Id, user.Email.Address, user.Role.ToString());
			response.RefreshToken = _refreshToken.GenerateRefreshToken(user);
			await unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("User with email {Email} logged in successfully.", request.Email);
			_logger.LogDebug("JWT Token: {Token}", response.AccessToken);
			_logger.LogDebug("Refresh Token: {RefreshToken}", response.RefreshToken);

			return response;
		}
	}
}
