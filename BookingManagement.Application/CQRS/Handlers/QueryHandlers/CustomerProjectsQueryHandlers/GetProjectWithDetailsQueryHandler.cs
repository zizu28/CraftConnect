using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetProjectWithDetailsQueryHandler(
		ICustomerProjectRepository repository,
		IMapper mapper,
		ILoggingService<GetProjectWithDetailsQueryHandler> logger) : IRequestHandler<GetProjectWithDetailsQuery, CustomerProjectResponseDTO?>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IMapper _mapper = mapper;
		private readonly ILoggingService<GetProjectWithDetailsQueryHandler> _logger = logger;

		public async Task<CustomerProjectResponseDTO?> Handle(GetProjectWithDetailsQuery request, CancellationToken cancellationToken)
		{
			var project = await _repository.GetProjectWithDetailsAsync(request.ProjectId, cancellationToken);

			if (project == null)
			{
				_logger.LogWarning("Project {ProjectId} details not found.", request.ProjectId);
				return null;
			}

			return _mapper.Map<CustomerProjectResponseDTO>(project);
		}
	}
}