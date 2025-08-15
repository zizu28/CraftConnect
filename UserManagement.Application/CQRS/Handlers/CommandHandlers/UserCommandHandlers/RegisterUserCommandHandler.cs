using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Events;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
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
		IBackgroundJobService backgroundJob)
		: IRequestHandler<RegisterUserCommand, UserResponseDTO>
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
				logger.LogInformation("Creating new user with email {Email}.", request.User!.Email);
				request.User!.Password = BCrypt.Net.BCrypt.HashPassword(
					request.User.Password!, 
					salt: BCrypt.Net.BCrypt.GenerateSalt(12));

				var newUser = mapper.Map<User>(request.User);
				newUser.PasswordHash = request.User.Password;

				await user.AddAsync(newUser, cancellationToken);
				var userRegisteredEvent = new UserRegisteredIntegrationEvent(newUser.Id, newUser.Email, newUser.Role);
				await eventBus.PublishAsync(userRegisteredEvent, cancellationToken);

				await user.SaveChangesAsync(cancellationToken);

				backgroundJob.Enqueue<IGmailService>(
					"default",
					email => email.SendEmailAsync(
						newUser.Email.Address,
						$"Welcome {newUser.FirstName}",
						$"A welcome message to {newUser.FirstName} using Asp.Nt Core.",
						false,
						CancellationToken.None
					)
				);			

				userResponse.UserId = newUser.Id;
				userResponse.Email = newUser.Email.Address;
				userResponse.PhoneNumber = newUser.Phone.Number;
				userResponse.Role = newUser.Role.ToString();
				userResponse.CreatedAt = DateTime.UtcNow;
				userResponse.Message = "User registration successful.";
				//await cacheService.SetAsync($"user:{userResponse.UserId}", userResponse, cancellationToken);
				return userResponse;
			}
			
			logger.LogWarning("User registration validation failed: {Errors}", validationResult.Errors);
			userResponse.Message = "User registration failed due to validation errors.";
			userResponse.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
			return userResponse;
		}
	}
}
