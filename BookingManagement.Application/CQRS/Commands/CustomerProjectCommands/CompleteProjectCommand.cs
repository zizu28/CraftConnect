using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class CompleteProjectCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public Guid CustomerId { get; set; }
		// Could include rating/review data here later
	}
}
