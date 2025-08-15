using MediatR;
using UserManagement.Application.DTOs.CustomerDTO;

namespace UserManagement.Application.CQRS.Queries.CustomerQueries
{
	public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerResponseDTO>>
	{
	}
}
