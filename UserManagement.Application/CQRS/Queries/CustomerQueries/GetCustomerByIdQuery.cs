using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.CustomerQueries
{
	public class GetCustomerByIdQuery : IRequest<CustomerResponseDTO>
	{
		public Guid CustomerId { get; set; }
	}
}
