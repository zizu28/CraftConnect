using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using BookingManagement.Application.DTOs.CustomerProjectDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class SearchOpenProjectsQueryHandler(ICustomerProjectRepository repository, IMapper mapper) : IRequestHandler<SearchOpenProjectsQuery, List<CustomerProjectResponseDTO>>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IMapper _mapper = mapper;

		public async Task<List<CustomerProjectResponseDTO>> Handle(SearchOpenProjectsQuery request, CancellationToken cancellationToken)
		{
			var projects = await _repository.SearchOpenProjectsAsync(
				request.SearchTerm,
				request.Skills,
				request.PageNumber,
				request.PageSize,
				cancellationToken);

			return _mapper.Map<List<CustomerProjectResponseDTO>>(projects);
		}
	}
}