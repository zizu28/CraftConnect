using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetAllProjectsQueryHandler(
		ILoggingService<GetAllCustomerProjectsQueryHandler> logger,
		ICustomerProjectRepository customerProjectRepository,
		IMapper mapper) : IRequestHandler<GetAllProjectsQuery, List<CustomerProjectResponseDTO>>
	{
		private readonly ILoggingService<GetAllCustomerProjectsQueryHandler> _logger = logger;
		private readonly ICustomerProjectRepository _customerProjectRepository = customerProjectRepository;
		private readonly IMapper _mapper = mapper;

		public async Task<List<CustomerProjectResponseDTO>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
		{
			var allProjects = await _customerProjectRepository.GetAllAsync(cancellationToken);
			if(allProjects is null || !allProjects.Any())
			{
				_logger.LogInformation("No customer projects found.");
				return [];
			}
			var projectDTOs = _mapper.Map<List<CustomerProjectResponseDTO>>(allProjects);
			_logger.LogInformation("Retrieved {Count} customer projects.", projectDTOs.Count);
			return projectDTOs;
		}
	}
}
