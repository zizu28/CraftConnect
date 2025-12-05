using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.IntegrationEvents.AllUserActivitiesIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using System.Net;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.Validators.CraftmanValidators;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class RegisterCraftmanCommandHandler(
		IMapper mapper,
		ICraftsmanRepository craftmanRepository,
		ApplicationDbContext dbContext, 
		ILoggingService<RegisterCraftmanCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker,
		IUnitOfWork unitOfWork) : IRequestHandler<RegisterCraftmanCommand, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(RegisterCraftmanCommand request, CancellationToken cancellationToken)
		{
			var response = new CraftmanResponseDTO();
			var validator = new CraftmanCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.Craftman, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				response.IsSuccessful = false;
				response.Message = "Craftman registration failed due to validation errors.";
				return response;
			}
			logger.LogInformation("Customer registration validation succeeded.");
			
			var craftman = await craftmanRepository.GetByEmailAsync(request.Craftman.Email, cancellationToken);
			if (craftman != null)
			{
				logger.LogWarning("Craftman with email {Email} already exists.", request.Craftman.Email);
				response.Message = "Craftman with this email already exists.";
				return response;
			}

			request.Craftman.Password = BCrypt.Net.BCrypt.HashPassword(
					request.Craftman.Password!,
					salt: BCrypt.Net.BCrypt.GenerateSalt(12));

			var newUser = mapper.Map<Craftman>(request.Craftman);
			newUser.PasswordHash = request.Craftman.Password;
			var verificationTokenValue = EmailVerificationToken.GenerateToken();
			var hashedToken = BCrypt.Net.BCrypt.HashPassword(verificationTokenValue);
			
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				logger.LogInformation("Creating new craftman with email {Email}.", request.Craftman.Email);
				
				await craftmanRepository.AddAsync(newUser, cancellationToken);

				var userRegisteredEvent = new UserRegisteredIntegrationEvent
				(
					newUser.Id,
					newUser.Email,
					newUser.Role
				);
				await messageBroker.PublishAsync(userRegisteredEvent, cancellationToken);

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
			}, cancellationToken);


			string? verificationLink = $"https://localhost:7235/api/users/confirm-email?token={hashedToken}";

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
						$"Email Verification for CraftConnect",
						$"A welcome message. " +
						$"To verify your email address, <a href='{verificationLink}'>click here</a>.",
						true,
						CancellationToken.None
					)
				);
			}

			response.EmailAddress = newUser.Email.Address;
			logger.LogInformation("Craftman with email {Email} registered successfully.", newUser.Email.Address);
			response.Message = "Customer registration successful.";
			response.IsSuccessful = true;
			//await cacheService.SetAsync($"user:{userResponse.UserId}", userResponse, cancellationToken);
			return response;
		}
	}
}
