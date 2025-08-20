using AutoMapper;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CustomerQueries;
using UserManagement.Application.DTOs.CustomerDTO;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CustomerQueryHandlers
{
	public class GetCustomerByAddressQueryHandler(
		IMapper mapper, ICustomerRepository customerRepository)
		: IRequestHandler<GetCustomerByAddressQuery, CustomerResponseDTO>
	{
		public async Task<CustomerResponseDTO> Handle(GetCustomerByAddressQuery request, CancellationToken cancellationToken)
		{
			var customer = await customerRepository.GetCustomerByAddressAsync(
				cust => cust.PostalCode == request.CustomerAddress.PostalCode
				&& cust.Street == request.CustomerAddress.Street
				&& cust.City == request.CustomerAddress.City, cancellationToken)
				?? throw new NotFoundException($"Customer with address " +
				$"\"{request.CustomerAddress.Street}, {request.CustomerAddress.City}, {request.CustomerAddress.PostalCode}\" not found.");
			var newCustomer = mapper.Map<CustomerResponseDTO>(customer);
			newCustomer.IsSuccess = true;
			return newCustomer;
		}
	}
}
