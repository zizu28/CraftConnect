using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	/// <summary>
	/// Command for changing password while authenticated (requires old password verification)
	/// </summary>
	public class ChangePasswordWhileAuthenticatedCommand : IRequest<Unit>
	{
		public Guid UserId { get; set; }
		public string OldPassword { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
