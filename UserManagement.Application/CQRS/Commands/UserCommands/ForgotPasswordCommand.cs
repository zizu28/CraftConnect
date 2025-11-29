using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class ForgotPasswordCommand : IRequest
	{
		public string Email { get; set; }
	}
}
