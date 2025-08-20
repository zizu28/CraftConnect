using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RegisterCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public CraftmanCreateDTO Craftman { get; set; }
	}
}
