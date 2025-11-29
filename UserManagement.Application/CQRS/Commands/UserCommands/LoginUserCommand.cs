using MediatR;
using UserManagement.Application.Responses;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class LoginUserCommand : IRequest<LoginResponse>
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string RedirectUri { get; set; } = string.Empty;
		public bool RememberMe { get; set; } 
	}
}
