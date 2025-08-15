using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class DeleteUserCommand : IRequest<Unit>
	{
		public Guid UserId { get; set; }
	}
}
