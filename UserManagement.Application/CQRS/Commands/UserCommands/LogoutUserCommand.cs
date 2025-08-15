using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class LogoutUserCommand : IRequest<Unit>
	{
		public string Username { get; set; } = string.Empty;
	}
}
