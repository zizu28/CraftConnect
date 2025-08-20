using AutoMapper;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CustomerQueries;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CustomerQueryHandlers
{
	public class GetAllCustomersQueryHandler(
		IMapper mapper, ICustomerRepository customerRepository)
		: IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerResponseDTO>>
	{
		public async Task<IEnumerable<CustomerResponseDTO>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
		{
			var customers = await customerRepository.GetAllAsync(cancellationToken)
				?? throw new NotFoundException("No customers found in database or cache.");
			var newCustomers = mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
			foreach (var customer in newCustomers)
			{
				customer.IsSuccess = true;
			}
			return newCustomers;
		}
	}
}
