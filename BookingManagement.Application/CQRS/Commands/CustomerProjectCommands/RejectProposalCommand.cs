using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class RejectProposalCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public Guid CustomerId { get; set; }
		public Guid ProposalId { get; set; }
	}
}
