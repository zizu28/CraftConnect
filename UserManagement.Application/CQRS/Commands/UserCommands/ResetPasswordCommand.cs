using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	/// <summary>
	/// Command for resetting password via email token (forgot password flow)
	/// </summary>
	public class ResetPasswordCommand : IRequest<Unit>
	{
		public string Email { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
