using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands
{
	public class WithdrawProposalCommand : IRequest<bool>
	{
		public Guid ProposalId { get; set; }
		public Guid CraftsmanId { get; set; }
	}
}