using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.AllUserActivitiesIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.Validators.UserValidators;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class RegisterUserCommandHandler(
		IMapper mapper, 
		ILoggingService<RegisterUserCommandHandler> logger, 
		IUserRepository user,
		IMessageBroker eventBus, 
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork,
		ApplicationDbContext dbContext) : IRequestHandler<RegisterUserCommand, UserResponseDTO>
	{
		public async Task<UserResponseDTO> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			var userResponse = new UserResponseDTO();
			var userValidator = new UserCreateDTOValidator();
			var validationResult = await userValidator.ValidateAsync(request.User!, cancellationToken);
			if(validationResult.IsValid)
			{
				logger.LogInformation("User registration validation succeeded.");
				var userEntity = await user.GetByEmailAsync(request.User!.Email, cancellationToken);
				if(userEntity != null)
				{
					logger.LogWarning("User with email {Email} already exists.", request.User!.Email);
					userResponse.Message = "User with this email already exists.";
					return userResponse;
				}
				var newUser = mapper.Map<User>(request.User);
				request.User!.Password = BCrypt.Net.BCrypt.HashPassword(
						request.User.Password!,
						salt: BCrypt.Net.BCrypt.GenerateSalt(12)
					);

				newUser.PasswordHash = request.User.Password;
				var verificationTokenValue = EmailVerificationToken.GenerateToken();
				var hashedToken = BCrypt.Net.BCrypt.HashPassword(verificationTokenValue);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					logger.LogInformation("Creating new user with email {Email}.", request.User!.Email);					

					await user.AddAsync(newUser, cancellationToken);
					var userRegisteredEvent = new UserRegisteredIntegrationEvent(newUser.Id, newUser.Email, newUser.Role);
					await eventBus.PublishAsync(userRegisteredEvent, cancellationToken);

					var utcNow = DateTime.UtcNow;
					var emailVerificationToken = new EmailVerificationToken
					{
						EmailVerificationTokenId = Guid.NewGuid(),
						TokenValue = hashedToken,
						User = newUser,
						UserId = newUser.Id,
						CreatedOnUtc = utcNow,
						ExpiresOnUtc = utcNow.AddDays(1)
					};
					dbContext.EmailVerificationTokens.Add(emailVerificationToken);

					userResponse.UserId = newUser.Id;
					userResponse.Email = newUser.Email.Address;
					userResponse.Role = newUser.Role.ToString();
					userResponse.CreatedAt = DateTime.UtcNow;
					userResponse.Message = "User registration successful.";
				}, cancellationToken);

				string? verificationLink = $"https://localhost:7272/users/confirm-email?token={hashedToken}";

				if (string.IsNullOrEmpty(verificationLink))
				{
					logger.LogError(new Exception(), "Failed to generate email confirmation link");
				}
				else 
				{
					backgroundJob.Enqueue<IGmailService>(
						"default",
						email => email.SendEmailAsync(
							newUser.Email.Address,
							$"Emai Verification for CraftConnect",
							$"A welcome message. " +
							$"To verify your email address, <a href='{verificationLink}'>click here</a>.",
							true,
							CancellationToken.None
						)
					);
				}
					
				return userResponse;
			}
			
			logger.LogWarning("User registration validation failed: {Errors}", validationResult.Errors);
			userResponse.Message = "User registration failed due to validation errors.";
			userResponse.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
			return userResponse;
		}
	}
}
