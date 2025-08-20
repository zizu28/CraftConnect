using AutoMapper;
using Infrastructure.BackgroundJobs;
using Infrastructure.Cache;
using Infrastructure.EmailService;
using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CustomerCommands;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.Validators.CustomerValidators;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CustomerCommandHandlers
{
	public class UpdateCustomerCommandHandler(
		IMapper mapper,
		ICustomerRepository customerRepository,
		ILogger<UpdateCustomerCommandHandler> logger,
		IBackgroundJobService backgroundJob) : IRequestHandler<UpdateCustomerCommand, CustomerResponseDTO>
	{
		public async Task<CustomerResponseDTO> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
		{
			var response = new CustomerResponseDTO();
			var validator = new CustomerUpdatDTOValidator();
			var validationResult = await validator.ValidateAsync(request.CustomerDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Validation failed for UpdateCustomerCommand: {Errors}", validationResult.Errors);
				response.Message = "Validation failed";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			var customer = await customerRepository.FindBy(c => c.Id == request.CustomerID, cancellationToken)
				?? throw new KeyNotFoundException($"Customer with ID {request.CustomerID} not found.");
			mapper.Map(request.CustomerDTO, customer);
			await customerRepository.UpdateAsync(customer, cancellationToken);
			//await cacheService.RemoveSync($"customer:{request.CustomerID}", cancellationToken);
			//await cacheService.SetAsync($"customer:{request.CustomerID}", 
				//mapper.Map<CustomerResponseDTO>(customer), cancellationToken);
			response.Message = "Customer updated successfully";
			response.IsSuccess = true;
			backgroundJob.Enqueue<IEmailService>("Update-Customer-Job",
				email => email.SendEmailAsync(
					customer.Email.Address, 
					$"Update {customer.FirstName} {customer.LastName}",
					"",
					true,
					CancellationToken.None));
			return response;
		}
	}
}
