using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class CreateCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public CraftmanCreateDTO CraftmanDTO { get; set; }
	}
}
