using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Commands.UserCommands
{
	public class RegisterCraftmanCommand : IRequest<CraftmanResponseDTO>
	{
		public CraftmanCreateDTO Craftman { get; set; }
	}
}
