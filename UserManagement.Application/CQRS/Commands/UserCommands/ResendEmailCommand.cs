using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class ResendEmailCommand : IRequest<Unit>
	{
		public string Email { get; set; }
	}
}
