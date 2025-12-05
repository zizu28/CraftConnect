using BookingManagement.Application.DTOs.CustomerProjectDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetAllCustomerProjectsQuery : IRequest<List<CustomerProjectResponseDTO>>
	{
	}
}
