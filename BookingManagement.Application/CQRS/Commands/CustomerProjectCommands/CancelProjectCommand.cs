using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class CancelProjectCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public Guid CustomerId { get; set; }
		public string Reason { get; set; } = string.Empty;
	}
}
