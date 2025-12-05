using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries;
using BookingManagement.Application.DTOs.CraftmanProposalDTOs;
using Core.Logging;
using Core.SharedKernel.Contracts;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CraftsmanProposalsQueryHandlers
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
			var names = await _userService.GetCraftsmanNamesAsync(craftsmanIds, cancellationToken);
			foreach (var dto in proposalDtos)
			{
				if (names.TryGetValue(dto.CraftsmanId, out var name))
				{
					dto.CraftsmanName = name;
				}
				else
				{
					dto.CraftsmanName = "Unknown Craftsman";
				}
			}
			return proposalDtos;
		}
	}
}
