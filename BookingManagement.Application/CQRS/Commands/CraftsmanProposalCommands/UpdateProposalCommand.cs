using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands
{
	public class UpdateProposalCommand : IRequest<bool>
	{
		public Guid ProposalId { get; set; }
		public Guid CraftsmanId { get; set; }
		public UpdateCraftsmanProposalDTO Data { get; set; } = new();
	}
}