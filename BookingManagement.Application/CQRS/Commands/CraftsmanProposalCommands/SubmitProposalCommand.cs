using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands
{
	public class SubmitProposalCommand : IRequest<Guid>
	{
		public Guid CraftsmanId { get; set; } // From Token
		public CreateCraftsmanProposalDTO Data { get; set; } = new();
	}
}