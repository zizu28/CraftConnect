using MediatR;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.CQRS.Commands.CustomerCommands
{
	public class UpdateCustomerCommand : IRequest<CustomerResponseDTO>
	{
		public Guid CustomerID { get; set; }
		public CustomerUpdateDTO CustomerDTO { get; set; }		
	}
}
