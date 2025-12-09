using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetAllProjectsQuery : IRequest<List<CustomerProjectResponseDTO>>
	{
	}
}
