using AutoMapper;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Validators.CraftmanValidators;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CraftmanCommandHandlers
{
	public class UpdateCraftmanCommandHandler(
		ICraftsmanRepository craftmanRepository,
		IMapper mapper,
		ILogger<UpdateCraftmanCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateCraftmanCommand, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(UpdateCraftmanCommand request, CancellationToken cancellationToken)
		{
			var craftman = await craftmanRepository.GetByIdAsync(request.CraftmanId, cancellationToken)
				?? throw new KeyNotFoundException($"Craftman with ID {request.CraftmanId} not found.");
			var response = new CraftmanResponseDTO();
			var validator = new CraftmanUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.CraftmanDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				response.IsSuccessful = false;
				response.Message = "Craftman creation failed due to validation errors.";
				return response;
			}
			
			craftman = mapper.Map(request.CraftmanDTO, craftman);
			await unitOfWork.ExecuteInTransactionAsync(
				async () => await craftmanRepository.UpdateAsync(craftman, cancellationToken), 
				cancellationToken
			);

			var mappedResponse = mapper.Map<CraftmanResponseDTO>(craftman);
			mappedResponse.IsSuccessful = true;
			mappedResponse.Message = "Craftman updated successfully.";
			//await cacheService.RemoveSync($"Craftman:{craftman.Id}", cancellationToken);
			//await cacheService.SetAsync($"Craftman:{request.CraftmanId}", response, cancellationToken);
			logger.LogInformation("Craftman with ID {CraftmanId} updated successfully.", craftman.Id);
			backgroundJob.Enqueue<IEmailService>("",
				emailService => emailService.SendEmailAsync(
					craftman.Email.Address,
					$"{craftman.FirstName} {craftman.LastName} Updated",
					"Your profile has been updated. If you did not issue this request, kindly contact us for immediate action.",
					true, CancellationToken.None));
			return mappedResponse;
		}
	}
}
