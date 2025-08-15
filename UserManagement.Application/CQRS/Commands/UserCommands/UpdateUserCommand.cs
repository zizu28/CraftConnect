using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class UpdateUserCommand : IRequest<Unit>
	{
		public string Username { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public bool IsAdmin { get; set; }
	}
}
