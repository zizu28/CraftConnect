using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.Logging;
using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetProjectsWithIncomingProposalsQueryHandler(
		ILoggingService<GetProjectsWithIncomingProposalsQueryHandler> logger,
		ICustomerProjectRepository customerProjectRepository,
		ICraftsmanProposalRepository craftsmanProposalRepository,
		IMapper mapper,
		IUserModuleService userModuleService) : IRequestHandler<GetProjectsWithIncomingProposalsQuery, List<CraftsmanProposalResponseDTO>>
	{
		private readonly ILoggingService<GetProjectsWithIncomingProposalsQueryHandler> _logger = logger;
		private readonly ICustomerProjectRepository _customerProjectRepository = customerProjectRepository;
		private readonly ICraftsmanProposalRepository _craftsmanProposalRepository = craftsmanProposalRepository;
		private readonly IMapper _mapper = mapper;
		private readonly IUserModuleService _userModuleService = userModuleService;

		public async Task<List<CraftsmanProposalResponseDTO>> Handle(GetProjectsWithIncomingProposalsQuery request, CancellationToken cancellationToken)
		{
			var projects = await _customerProjectRepository.GetProjectsByCustomerAsync(request.CustomerId, cancellationToken);
			var projectIds = projects.Select(p => p.Id).ToList();
			if (projectIds.Count == 0)
			{
				return [];
			}
			var proposals = await _craftsmanProposalRepository.GetProposalsByProjectIdsAsync(projectIds, cancellationToken);
			var dtos = _mapper.Map<List<CraftsmanProposalResponseDTO>>(proposals);
			var names = await _userModuleService.GetCraftsmanNamesAsync(dtos.Select(x => x.CraftsmanId), cancellationToken);
			var projectMap = projects.ToDictionary(p => p.Id, p => p.Title);
			foreach (var dto in dtos)
			{
				if (names.TryGetValue(dto.CraftsmanId, out var name))
				{
					dto.CraftsmanName = name;
				}
				if (projectMap.TryGetValue(dto.ProjectId, out var title))
				{
					dto.ProjectTitle = title;
				}
			}
			_logger.LogInformation("Retrieved {Count} incoming proposals for Customer {CustomerId}", dtos.Count, request.CustomerId);
			return dtos;
		}
	}
}
