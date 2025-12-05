using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RegisterUserCommand : IRequest<UserResponseDTO>
	{
		public UserCreateDTO? User { get; set; }
	}
}
