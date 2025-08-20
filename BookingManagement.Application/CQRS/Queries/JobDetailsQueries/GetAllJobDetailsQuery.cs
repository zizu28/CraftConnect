using BookingManagement.Application.DTOs.JobDetailsDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.JobDetailsQueries
{
	public class GetAllJobDetailsQuery : IRequest<IEnumerable<JobDetailsResponseDTO>>
	{
	}
}
