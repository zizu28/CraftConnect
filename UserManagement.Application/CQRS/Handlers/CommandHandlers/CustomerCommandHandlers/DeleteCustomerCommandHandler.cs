using Infrastructure.Cache;
using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CustomerCommands;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CustomerCommandHandlers
{
	public class DeleteCustomerCommandHandler(
		ICustomerRepository customerRepository,
		ILogger<DeleteCustomerCommandHandler> logger) : IRequestHandler<DeleteCustomerCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
		{
			var customer = await customerRepository.FindBy(c => c.Id == request.CustomerID, cancellationToken)
				?? throw new KeyNotFoundException($"Customer with ID {request.CustomerID} not found.");
			await customerRepository.DeleteAsync(customer.Id, cancellationToken);
			//await cacheService.RemoveSync($"customer:{request.CustomerID}", cancellationToken);
			logger.LogInformation("Customer with ID {CustomerID} deleted successfully.", request.CustomerID);
			return Unit.Value;
		}
	}
}
