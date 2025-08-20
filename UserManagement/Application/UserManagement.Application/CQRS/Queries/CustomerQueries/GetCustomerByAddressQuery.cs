using MediatR;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.CQRS.Queries.CustomerQueries
{
	public class GetCustomerByAddressQuery : IRequest<CustomerResponseDTO>
	{
		public CustomerAddressDTO CustomerAddress { get; set; }
	}
}
