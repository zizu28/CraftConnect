using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RefreshTokenCommand : IRequest<(string AccessToken, string RefreshToken)>
	{
		public string RefreshToken { get; set; }
	}
}
