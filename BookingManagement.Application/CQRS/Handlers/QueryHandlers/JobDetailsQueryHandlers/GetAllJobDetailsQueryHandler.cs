using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.JobDetailsQueries;
using BookingManagement.Application.DTOs.JobDetailsDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.JobDetailsQueryHandlers
{
	public class GetAllJobDetailsQueryHandler(
		IJobDetailsRepository jobDetailsRepository,
		IMapper mapper) : IRequestHandler<GetAllJobDetailsQuery, IEnumerable<JobDetailsResponseDTO>>
	{
		public async Task<IEnumerable<JobDetailsResponseDTO>> Handle(GetAllJobDetailsQuery request, CancellationToken cancellationToken)
		{
			var jobDetails = await jobDetailsRepository.GetAllAsync(cancellationToken)
				?? throw new InvalidOperationException("No job details found.");
			return mapper.Map<IEnumerable<JobDetailsResponseDTO>>(jobDetails);
		}
	}
}
