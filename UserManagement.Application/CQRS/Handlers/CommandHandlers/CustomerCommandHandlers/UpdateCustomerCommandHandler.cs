using AutoMapper;
using Core.SharedKernel.DTOs;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CustomerCommands;
using UserManagement.Application.Validators.CustomerValidators;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CustomerCommandHandlers
{
	public class UpdateCustomerCommandHandler(
		IMapper mapper,
		ICustomerRepository customerRepository,
		ILogger<UpdateCustomerCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateCustomerCommand, CustomerResponseDTO>
	{
		private readonly IMapper _mapper = mapper;
		private readonly ICustomerRepository _customerRepository = customerRepository;
		private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
		private readonly IBackgroundJobService _backgroundJob = backgroundJob;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<CustomerResponseDTO> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
		{
			var response = new CustomerResponseDTO();
			var validator = new CustomerUpdatDTOValidator();
			var validationResult = await validator.ValidateAsync(request.CustomerDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validation failed for UpdateCustomerCommand: {Errors}", validationResult.Errors);
				response.Message = "Validation failed";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}

			var customer = await _customerRepository.FindBy(c => c.Id == request.CustomerID, cancellationToken)
				?? throw new KeyNotFoundException($"Customer with ID {request.CustomerID} not found.");

			try
			{
				_mapper.Map(request.CustomerDTO, customer);

				await _unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await _customerRepository.UpdateAsync(customer, cancellationToken);
				}, cancellationToken);

				var responseDto = _mapper.Map<CustomerResponseDTO>(customer);
				responseDto.IsSuccess = true;
				responseDto.Message = "Customer updated successfully";

				_backgroundJob.Enqueue<IGmailService>(
					"default",
					email => email.SendEmailAsync(
						customer.Email.Address,
						$"Profile Updated",
						$"Hello {customer.FirstName}, your profile details have been updated.",
						true,
						CancellationToken.None));

				return responseDto;
			}
			catch (DbUpdateConcurrencyException)
			{
				_logger.LogError("Concurrency conflict for Customer {Id}", request.CustomerID);
				return new CustomerResponseDTO
				{
					IsSuccess = false,
					Message = "The record was modified by another user. Please reload."
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating Customer {Id}", request.CustomerID);
				return new CustomerResponseDTO
				{
					IsSuccess = false,
					Message = "An unexpected error occurred."
				};
			}
		}
	}
}
