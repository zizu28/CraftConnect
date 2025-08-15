using AutoMapper;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CustomerQueries;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CustomerQueryHandlers
{
	public class GetCustomerByIdQueryHandler(
		IMapper mapper, ICustomerRepository customerRepository)
		: IRequestHandler<GetCustomerByIdQuery, CustomerResponseDTO>
	{
		public async Task<CustomerResponseDTO> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
		{
			var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
				?? throw new NotFoundException($"Customer with ID {request.CustomerId} not found in database or cache.");
			var newCustomer = mapper.Map<CustomerResponseDTO>(customer);
			newCustomer.IsSuccess = true;
			return newCustomer;
		}
	}
}
