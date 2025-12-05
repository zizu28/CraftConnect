using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Commands.CustomerCommands
{
	public class UpdateCustomerCommand : IRequest<CustomerResponseDTO>
	{
		public Guid CustomerID { get; set; }
		public CustomerUpdateDTO CustomerDTO { get; set; }		
	}
}
