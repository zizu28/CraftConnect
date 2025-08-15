using Core.Logging;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Validators.UserValidators;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class LoginUserCommandHandler(
		ILoggingService<LoginUserCommandHandler> _logger,
		IUserRepository _user, 
		ITokenProvider _refreshToken) 
		: IRequestHandler<LoginUserCommand, (string AccesToken, string RefreshToken)>
	{

		public async Task<(string AccesToken, string RefreshToken)> Handle(LoginUserCommand request, CancellationToken cancellationToken)
		{
			var accessToken = string.Empty;
			var refreshToken = string.Empty;
			var validator = new LoginUserCommandValidator();
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validation failed for login request");
				return (accessToken, refreshToken);
			}
			var user = await _user.FindBy(user => user.Username.Equals(request.Username), cancellationToken)
				?? throw new KeyNotFoundException($"User with username {request.Username} not found.");
			var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
			if(!isPasswordValid)
			{
				_logger.LogWarning("Invalid login attempt for user: {Username}", request.Username);
				return (accessToken, refreshToken);
			}
			accessToken = _refreshToken.GenerateAccessToken(user.Id, user.Email.Address, user.Role.ToString());
			refreshToken = await _refreshToken.GenerateRefreshToken(user);
			
			_logger.LogInformation("User {Username} logged in successfully.", request.Username);
			_logger.LogInformation("Generated JWT and refresh tokens for user: {Username}", request.Username);
			_logger.LogDebug("JWT Token: {Token}", accessToken);
			_logger.LogDebug("Refresh Token: {RefreshToken}", refreshToken);

			return (accessToken, refreshToken);
		}
	}
}
