using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries;
using Core.Logging;
using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CraftsmanProposalsQueryHandlers
{
	public class GetPendingProposalsByProjectQueryHandler(
		ICraftsmanProposalRepository repository,
		ILoggingService<GetPendingProposalsByProjectQueryHandler> logger,
		IUserModuleService userService,
		IMapper mapper) : IRequestHandler<GetPendingProposalsByProjectQuery, IEnumerable<CraftsmanProposalResponseDTO>>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly ILoggingService<GetPendingProposalsByProjectQueryHandler> _logger = logger;
		private readonly IUserModuleService _userService = userService;
		private readonly IMapper _mapper = mapper;
		public async Task<IEnumerable<CraftsmanProposalResponseDTO>> Handle(GetPendingProposalsByProjectQuery request, CancellationToken cancellationToken)
		{
			var proposals = await _repository.GetPendingProposalsByProjectAsync(request.ProjectId, cancellationToken);
			_logger.LogInformation("Retrieved pending craftsman proposals. Count: {Count}", proposals.Count());
			
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
