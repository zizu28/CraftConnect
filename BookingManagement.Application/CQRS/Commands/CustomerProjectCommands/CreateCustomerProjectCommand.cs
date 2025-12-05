using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Commands.CustomerProjectCommands
{
	public class CreateCustomerProjectCommand : IRequest<Guid>
	{
		public Guid CustomerId { get; set; }
		public CreateCustomerProjectDTO Data { get; set; } = new();
	}
}
