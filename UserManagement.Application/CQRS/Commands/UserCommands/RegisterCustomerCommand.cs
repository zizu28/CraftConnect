using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RegisterCustomerCommand : IRequest<CustomerResponseDTO>
	{
		public CustomerCreateDTO? Customer { get; set; }
	}
}
