using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetProjectsByStatusQueryHandler(ICustomerProjectRepository repository, IMapper mapper) : IRequestHandler<GetProjectsByStatusQuery, List<CustomerProjectResponseDTO>>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IMapper _mapper = mapper;

		public async Task<List<CustomerProjectResponseDTO>> Handle(GetProjectsByStatusQuery request, CancellationToken cancellationToken)
		{
			if (!Enum.TryParse<ServiceRequestStatus>(request.Status, true, out var statusEnum))
			{
				return []; // Or throw validation exception
			}

			var projects = await _repository.GetProjectsByStatusAsync(statusEnum, cancellationToken);
			return _mapper.Map<List<CustomerProjectResponseDTO>>(projects);
		}
	}
}