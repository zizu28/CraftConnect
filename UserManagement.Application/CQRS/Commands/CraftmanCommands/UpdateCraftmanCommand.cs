using Core.SharedKernel.DTOs;
using MediatR;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Commands.CraftmanCommands
{
	public class UpdateCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
		public CraftsmanProfileUpdateDTO CraftmanDTO { get; set; }
	}
}
