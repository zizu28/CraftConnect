using MediatR;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RegisterUserCommand : IRequest<UserResponseDTO>
	{
		public UserCreateDTO? User { get; set; }
	}
}
