using MediatR;
using UserManagement.Application.Responses;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class LoginUserCommand : IRequest<LoginResponse>
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}
}
