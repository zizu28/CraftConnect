using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class AcceptProposalCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public Guid ProposalId { get; set; }
		public Guid CraftsmanId { get; set; }
	}
}
