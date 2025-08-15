using MediatR;

namespace UserManagement.Application.CQRS.Commands.CustomerCommands
{
	public class DeleteCustomerCommand : IRequest<Unit>
	{
		public Guid CustomerID { get; set; }
	}
}
