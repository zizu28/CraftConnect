using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class ConfirmEmailCommand : IRequest<bool>
	{
		public string token { get; set; }
	}
}
