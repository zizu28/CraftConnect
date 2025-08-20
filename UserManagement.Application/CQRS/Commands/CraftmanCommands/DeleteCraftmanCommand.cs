using MediatR;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class DeleteCraftmanCommand : IRequest<Unit>
	{
		public Guid CraftmanId { get; set; }
	}
}
