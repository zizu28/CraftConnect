using MediatR;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class LoginUserCommand : IRequest<(string AccesToken, string RefreshToken)>
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
