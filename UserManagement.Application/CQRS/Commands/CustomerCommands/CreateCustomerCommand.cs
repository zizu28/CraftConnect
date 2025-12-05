using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Commands.CustomerCommands
{
	public class CreateCustomerCommand : IRequest<CustomerResponseDTO>
	{
		public CustomerCreateDTO Customer { get; set; }
	}
}
