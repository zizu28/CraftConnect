using BookingManagement.Application.DTOs.JobDetailsDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.JobDetailsQueries
{
	public class GetJobDetailsByIdQuery : IRequest<JobDetailsResponseDTO>
	{
		public Guid Id { get; set; }
	}
}
