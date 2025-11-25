using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.Validators.CustomerValidators;
using UserManagement.Domain.Entities;
using UserManagement.Domain.UserIntegrationEvents;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class RegisterCustomerCommandHandler(
		IMapper mapper,
		ILoggingService<RegisterCustomerCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		ICustomerRepository customerRepository, 
		ApplicationDbContext dbContext,
		IMessageBroker publisher,
		IUnitOfWork unitOfWork) : IRequestHandler<RegisterCustomerCommand, CustomerResponseDTO>
	{
		public async Task<CustomerResponseDTO> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
		{
			var response = new CustomerResponseDTO();
			var validator = new CustomerCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.Customer!, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning($"Validation failed for RegisterCustomerCommand: {validationResult.Errors}");
				response.IsSuccess = false;
				response.Message = "Validation failed";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			logger.LogInformation("Customer registration validation succeeded.");
			var customer = await customerRepository.GetByEmailAsync(request.Customer!.Email, cancellationToken);
			if (customer != null)
			{
				logger.LogWarning("Customer with email {Email} already exists.", request.Customer!.Email);
				response.Message = "Customer with this email already exists.";
				return response;
			}

			logger.LogInformation("Creating new customer with email {Email}.", request.Customer!.Email);
			request.Customer!.Password = BCrypt.Net.BCrypt.HashPassword(
				request.Customer.Password!,
				salt: BCrypt.Net.BCrypt.GenerateSalt(12));

			var newUser = mapper.Map<Customer>(request.Customer);
			newUser.PasswordHash = request.Customer.Password;
			var verificationTokenValue = EmailVerificationToken.GenerateToken();
			var hashedToken = BCrypt.Net.BCrypt.HashPassword(verificationTokenValue);

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				await customerRepository.AddAsync(newUser, cancellationToken);
				var userRegisteredEvent = new UserCreatedIntegrationEvent(newUser);
				await publisher.PublishAsync(userRegisteredEvent, cancellationToken);

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

			response.CustomerId = newUser.Id;
			response.Email = newUser.Email.Address;
			response.Message = "Customer registration successful.";
			response.IsSuccess = true;
			//var confirmationToken = await GenerateEmailConfirmationToken(newUser);
			return response;
		}
	}
}
