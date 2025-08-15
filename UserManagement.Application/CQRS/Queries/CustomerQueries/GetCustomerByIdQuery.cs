using MediatR;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.CQRS.Queries.CustomerQueries
{
	public class GetCustomerByIdQuery : IRequest<CustomerResponseDTO>
	{
		public Guid CustomerId { get; set; }
	}
}
