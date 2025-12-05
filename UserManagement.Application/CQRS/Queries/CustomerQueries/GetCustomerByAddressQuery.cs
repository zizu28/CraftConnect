using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.CustomerQueries
{
	public class GetCustomerByAddressQuery : IRequest<CustomerResponseDTO>
	{
		public CustomerAddressDTO CustomerAddress { get; set; }
	}
}
