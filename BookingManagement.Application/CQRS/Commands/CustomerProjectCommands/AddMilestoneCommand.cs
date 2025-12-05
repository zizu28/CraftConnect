using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class AddMilestoneCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public string Title { get; set; } = string.Empty;
		public DateTime DueDate { get; set; }
	}
}
