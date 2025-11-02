using Core.Logging;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Responses;
using UserManagement.Application.Validators.UserValidators;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class LoginUserCommandHandler(
		ILoggingService<LoginUserCommandHandler> _logger,
		IUserRepository _user, 
		ITokenProvider _refreshToken) 
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
			var user = await _user.FindBy(user => user.Email.Address.Equals(request.Email), cancellationToken)
				?? throw new KeyNotFoundException($"User with email {request.Email} not found.");
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
			response.AccessToken = _refreshToken.GenerateAccessToken(user.Id, user.Email.Address, user.Role.ToString());
			response.RefreshToken = await _refreshToken.GenerateRefreshToken(user);
			
			_logger.LogInformation("User with email {Email} logged in successfully.", request.Email);
			_logger.LogInformation("Invalid login attempt for user with email: {Email}", request.Email);
			_logger.LogDebug("JWT Token: {Token}", response.AccessToken);
			_logger.LogDebug("Refresh Token: {RefreshToken}", response.RefreshToken);

			return response;
		}
	}
}
