using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetAllCustomerProjectsQuery : IRequest<List<CustomerProjectResponseDTO>>
	{
	}
}
