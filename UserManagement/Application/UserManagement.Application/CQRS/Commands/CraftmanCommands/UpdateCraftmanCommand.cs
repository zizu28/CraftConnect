using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class UpdateCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
		public CraftmanUpdateDTO CraftmanDTO { get; set; }
	}
}
