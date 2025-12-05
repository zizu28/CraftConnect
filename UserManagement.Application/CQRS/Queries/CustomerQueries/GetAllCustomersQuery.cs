using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.CustomerQueries
{
	public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerResponseDTO>>
	{
	}
}
