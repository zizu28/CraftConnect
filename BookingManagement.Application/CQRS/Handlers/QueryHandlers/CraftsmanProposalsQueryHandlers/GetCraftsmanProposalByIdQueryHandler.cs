using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries;
using Core.Logging;
using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CraftsmanProposalsQueryHandlers
{
	public class GetCraftsmanProposalByIdQueryHandler(
		ICraftsmanProposalRepository repository,
		ILoggingService<GetCraftsmanProposalByIdQueryHandler> logger,
		IUserModuleService userService,
		IMapper mapper) : IRequestHandler<GetCraftsmanProposalByIdQuery, CraftsmanProposalResponseDTO?>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly ILoggingService<GetCraftsmanProposalByIdQueryHandler> _logger = logger;
		private readonly IUserModuleService _userService = userService;
		private readonly IMapper _mapper = mapper;
		public async Task<CraftsmanProposalResponseDTO?> Handle(GetCraftsmanProposalByIdQuery request, CancellationToken cancellationToken)
		{
			var proposal = await _repository.GetByIdAsync(request.ProposalId, cancellationToken);
			if (proposal == null)
			{
				_logger.LogWarning("Craftsman proposal with ID {ProposalId} not found.", request.ProposalId);
				return null;
			}
			var proposalDto = _mapper.Map<CraftsmanProposalResponseDTO>(proposal);
			var name = await _userService.GetCraftsmanNameAsync(proposalDto.CraftsmanId, cancellationToken);
			proposalDto.CraftsmanName = name ?? "Unknown Craftsman";
			_logger.LogInformation("Retrieved craftsman proposal with ID {ProposalId}.", request.ProposalId);
			return proposalDto;
		}
	}
}
