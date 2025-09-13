using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.AllUserActivitiesIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Validators.CraftmanValidators;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers
{
	public class RegisterCraftmanCommandHandler(
		IMapper mapper,
		ICraftsmanRepository craftmanRepository,
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
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				logger.LogInformation("Creating new craftman with email {Email}.", request.Craftman.Email);
				request.Craftman.Password = BCrypt.Net.BCrypt.HashPassword(
					request.Craftman.Password!,
					salt: BCrypt.Net.BCrypt.GenerateSalt(12));

				var newUser = mapper.Map<Craftman>(request.Craftman);
				newUser.PasswordHash = request.Craftman.Password;
				foreach (var skillName in request.Craftman.Skills)
				{
					newUser.AddSkill(skillName, request.Craftman.YearsOfExperience);
				}
				await craftmanRepository.AddAsync(newUser, cancellationToken);

				var userRegisteredEvent = new UserRegisteredIntegrationEvent
				(
					newUser.Id,
					newUser.Email,
					newUser.Role
				);
				await messageBroker.PublishAsync(userRegisteredEvent, cancellationToken);

				backgroundJob.Enqueue<IGmailService>(
					"default",
					email => email.SendEmailAsync(
						newUser.Email.Address,
						$"Welcome {newUser.FirstName}",
						$"Welcome email to {newUser.FirstName} sent from Asp.Net Core.",
						true,
						CancellationToken.None
					)
				);

				response.Id = newUser.Id;
				response.FirstName = newUser.FirstName;
				response.LastName = newUser.LastName;
				response.Email = newUser.Email.Address;
				response.Phone = newUser.Phone.Number;
				response.Profession = newUser.Profession.ToString();
				response.Bio = newUser.Bio;
				response.HourlyRate = newUser.HourlyRate.Amount;
				response.Currency = newUser.HourlyRate.Currency;
				response.Status = newUser.Status.ToString();
				response.IsAvailable = newUser.IsAvailable;
				response.Skills = [.. newUser.Skills.Select(s => s.Name)];
				logger.LogInformation("Craftman with email {Email} registered successfully.", newUser.Email.Address);
				response.Message = "Customer registration successful.";
				response.IsSuccessful = true;
			}, cancellationToken);
			
			//await cacheService.SetAsync($"user:{userResponse.UserId}", userResponse, cancellationToken);
			return response;
		}
	}
}
