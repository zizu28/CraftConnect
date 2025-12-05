using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries;
using BookingManagement.Application.DTOs.CraftmanProposalDTOs;
using Core.Logging;
using Core.SharedKernel.Contracts;
using FluentEmail.Core;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CraftsmanProposalsQueryHandlers
{
	public class GetProposalsByCraftsmanQueryHandler(
		ICraftsmanProposalRepository repository,
		ILoggingService<GetProposalsByCraftsmanQueryHandler> logger,
		IUserModuleService userService,
		IMapper mapper) : IRequestHandler<GetProposalsByCraftsmanQuery, IEnumerable<CraftsmanProposalResponseDTO>>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly ILoggingService<GetProposalsByCraftsmanQueryHandler> _logger = logger;
		private readonly IUserModuleService _userService = userService;
		private readonly IMapper _mapper = mapper;
		public async Task<IEnumerable<CraftsmanProposalResponseDTO>> Handle(GetProposalsByCraftsmanQuery request, CancellationToken cancellationToken)
		{
			var proposals = await _repository.GetProposalsByCraftsmanAsync(request.CraftsmanId, cancellationToken);
			_logger.LogInformation("Retrieved proposals for CraftsmanId: {CraftsmanId}. Count: {Count}", request.CraftsmanId, proposals.Count());
			
			var proposalDtos = _mapper.Map<IEnumerable<CraftsmanProposalResponseDTO>>(proposals);
			var name = await _userService.GetCraftsmanNameAsync(request.CraftsmanId, cancellationToken)
			   ?? "Unknown Craftsman";
			proposalDtos.ForEach(dto => dto.CraftsmanName = name);
			return proposalDtos;
		}
	}
}
