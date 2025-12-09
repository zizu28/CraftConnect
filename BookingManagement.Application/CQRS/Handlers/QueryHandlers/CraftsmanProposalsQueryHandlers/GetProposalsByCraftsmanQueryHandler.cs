using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries;
using Core.Logging;
using Core.SharedKernel.Contracts;
using Core.SharedKernel.DTOs;
using FluentEmail.Core;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CraftsmanProposalsQueryHandlers
{
	public class GetProposalsByCraftsmanQueryHandler(
		ICraftsmanProposalRepository repository,
		ICustomerProjectRepository customerProjectRepository,
		ILoggingService<GetProposalsByCraftsmanQueryHandler> logger,
		IUserModuleService userService,
		IMapper mapper) : IRequestHandler<GetProposalsByCraftsmanQuery, IEnumerable<CraftsmanProposalResponseDTO>>
	{
		private readonly ICraftsmanProposalRepository _repository = repository;
		private readonly ICustomerProjectRepository _customerProjectRepository = customerProjectRepository;
		private readonly ILoggingService<GetProposalsByCraftsmanQueryHandler> _logger = logger;
		private readonly IUserModuleService _userService = userService;
		private readonly IMapper _mapper = mapper;
		public async Task<IEnumerable<CraftsmanProposalResponseDTO>> Handle(GetProposalsByCraftsmanQuery request, CancellationToken cancellationToken)
		{
			var proposals = await _repository.GetProposalsByCraftsmanAsync(request.CraftsmanId, cancellationToken);
			_logger.LogInformation("Retrieved proposals for CraftsmanId: {CraftsmanId}. Count: {Count}", request.CraftsmanId, proposals.Count());
			if(!proposals.Any())
			{
				return [];
			}
			var proposalDtos = _mapper.Map<IEnumerable<CraftsmanProposalResponseDTO>>(proposals);
			var name = await _userService.GetCraftsmanNameAsync(request.CraftsmanId, cancellationToken)
			   ?? "Me";
			proposalDtos.ForEach(dto => dto.CraftsmanName = name);
			var projectIds = proposalDtos.Select(d => d.ProjectId).Distinct().ToList();
			foreach(var dto in proposalDtos)
			{
				var project = await _customerProjectRepository.GetByIdAsync(dto.ProjectId, cancellationToken);
				if (project != null)
				{
					dto.ProjectTitle = project.Title;
					var clientName = await _userService.GetCustomerNameAsync(project.CustomerId, cancellationToken);
					dto.ClientName = clientName ?? "Unknown Client";
				}
				else
				{
					dto.ProjectTitle = "Project Not Found";
				}
			}
			return proposalDtos;
		}
	}
}
