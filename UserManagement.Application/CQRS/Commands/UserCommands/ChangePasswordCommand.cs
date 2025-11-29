using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class ChangePasswordCommand : IRequest<Unit>
	{
		public string Email { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
