using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using MediatR;
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
		ITokenProvider tokenProvider,
		IMassTransitIntegrationEventBus publisher) : IRequestHandler<RegisterCustomerCommand, CustomerResponseDTO>
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

			
			await customerRepository.AddAsync(newUser, cancellationToken);
			var userRegisteredEvent = new UserCreatedIntegrationEvent(newUser);
			await publisher.PublishAsync(userRegisteredEvent, cancellationToken);
			await customerRepository.SaveChangesAsync(cancellationToken);


			backgroundJob.Enqueue<IGmailService>(
				"default",
				email => email.SendEmailAsync(
					newUser.Email.Address,
					$"Welcome {newUser.FirstName}",
					$"This is a welcome message for {newUser.FirstName}",
					false,
					CancellationToken.None
				)
			);

			var emailConfirmationToken = tokenProvider.GenerateCustomerEmailConfirmationToken(newUser);
			backgroundJob.Enqueue<IGmailService>(
				"default",
				email => email.ConfirmEmailAsync(
					newUser.Email.Address,
					"Confirm Email",
					$"Confirm your email for {newUser.FirstName}. " +
					$"Please confirm your email by clicking the link: " +
					$"<a href='https://example.com/confirm-email?token={emailConfirmationToken}'>Confirm Email</a>"
					, cancellationToken)
			);

			response.CustomerId = newUser.Id;
			response.Email = newUser.Email.Address;
			response.Phone = newUser.Phone.Number;
			response.Address = newUser.Address?.ToString() ?? string.Empty;
			response.PreferredPaymentMethod = newUser.PreferredPaymentMethod.ToString();
			response.Message = "Customer registration successful.";
			response.IsSuccess = true;

			//var confirmationToken = await GenerateEmailConfirmationToken(newUser);
			return response;
		}

		private static async Task<string> GenerateEmailConfirmationToken(Customer customer)
		{
			throw new NotImplementedException();
		}
	}
}
