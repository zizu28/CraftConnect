using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class ChangePasswordCommand : IRequest<Unit>
	{
		public string Username { get; set; } = string.Empty;
		public string OldPassword { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
	}
}
