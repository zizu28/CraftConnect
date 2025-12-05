using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class UpdateCustomerProjectCommand : IRequest<bool>
	{
		public Guid ProjectId { get; set; }
		public Guid CustomerId { get; set; }
		public UpdateCustomerProjectDTO Data { get; set; } = new();
	}
}
