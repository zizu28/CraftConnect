using MediatR;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RegisterCustomerCommand : IRequest<CustomerResponseDTO>
	{
		public CustomerCreateDTO? Customer { get; set; }
	}
}
