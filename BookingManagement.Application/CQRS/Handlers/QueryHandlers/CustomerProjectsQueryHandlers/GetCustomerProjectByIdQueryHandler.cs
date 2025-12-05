using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetCustomerProjectByIdQueryHandler(
		ICustomerProjectRepository repository,
		IMapper mapper,
		ILoggingService<GetCustomerProjectByIdQueryHandler> logger) : IRequestHandler<GetCustomerProjectByIdQuery, CustomerProjectResponseDTO?>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IMapper _mapper = mapper;
		private readonly ILoggingService<GetCustomerProjectByIdQueryHandler> _logger = logger;

		public async Task<CustomerProjectResponseDTO?> Handle(GetCustomerProjectByIdQuery request, CancellationToken cancellationToken)
		{
			var project = await _repository.GetByIdAsync(request.ProjectId, cancellationToken);
			if (project == null)
			{
				_logger.LogWarning("Project {ProjectId} not found.", request.ProjectId);
				return null;
			}
			return _mapper.Map<CustomerProjectResponseDTO>(project);
		}
	}
}