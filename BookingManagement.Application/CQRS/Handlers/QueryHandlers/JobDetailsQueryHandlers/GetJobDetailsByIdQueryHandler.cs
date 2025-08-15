using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.JobDetailsQueries;
using BookingManagement.Application.DTOs.JobDetailsDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.JobDetailsQueryHandlers
{
	public class GetJobDetailsByIdQueryHandler(
		IJobDetailsRepository jobDetailsRepository,
		IMapper mapper) : IRequestHandler<GetJobDetailsByIdQuery, JobDetailsResponseDTO>
	{
		public async Task<JobDetailsResponseDTO> Handle(GetJobDetailsByIdQuery request, CancellationToken cancellationToken)
		{
			var jobDetail = await jobDetailsRepository.GetByIdAsync(request.Id, cancellationToken)
				?? throw new KeyNotFoundException($"Job details with ID {request.Id} not found.");
			return mapper.Map<JobDetailsResponseDTO>(jobDetail);
		}
	}
}
