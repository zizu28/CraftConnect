using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class CreateCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public CraftmanCreateDTO CraftmanDTO { get; set; }
	}
}
