using MediatR;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.CQRS.Commands.CustomerCommands
{
	public class CreateCustomerCommand : IRequest<CustomerResponseDTO>
	{
		public CustomerCreateDTO Customer { get; set; }
	}
}
