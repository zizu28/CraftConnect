using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class UpdateCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
		public CraftsmanProfileUpdateDTO CraftmanDTO { get; set; }
	}
}
