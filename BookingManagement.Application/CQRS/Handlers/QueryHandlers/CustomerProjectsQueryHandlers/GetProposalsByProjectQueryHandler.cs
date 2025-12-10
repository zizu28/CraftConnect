using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.Logging;
using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetProposalsByProjectQueryHandler(
		ICraftsmanProposalRepository repository,
		ILoggingService<GetProposalsByProjectQueryHandler> logger,
		IUserModuleService userService,
		IMapper mapper) : IRequestHandler<GetProposalsByProjectQuery, IEnumerable<CraftsmanProposalResponseDTO>>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly ILoggingService<GetProposalsByProjectQueryHandler> _logger = logger;
		private readonly IUserModuleService _userService = userService;
		private readonly IMapper _mapper = mapper;
		public async Task<IEnumerable<CraftsmanProposalResponseDTO>> Handle(GetProposalsByProjectQuery request, CancellationToken cancellationToken)
		{
			var proposals = await _repository.GetProposalsByProjectAsync(request.ProjectId, cancellationToken);
			_logger.LogInformation("Retrieved proposals for project {ProjectId}. Count: {Count}", request.ProjectId, proposals.Count());
			
			var proposalDtos = _mapper.Map<IEnumerable<CraftsmanProposalResponseDTO>>(proposals);
			var craftsmanIds = proposalDtos.Select(p => p.CraftsmanId).Distinct();
			var summaries = await _userService.GetCraftsmanSummariesAsync(craftsmanIds, cancellationToken);
			
			foreach (var dto in proposalDtos)
			{
				if (summaries.TryGetValue(dto.CraftsmanId, out var summary))
				{
					dto.CraftsmanName = summary.Name;
					dto.CraftsmanAvatarUrl = summary.AvatarUrl;
				}
				else
				{
					dto.CraftsmanName = "Unknown Craftsman";
					dto.CraftsmanAvatarUrl = "/images/user-avatar.jpg"; 
				}
			}
			return proposalDtos;
		}
	}
}
