using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetAllCustomerProjectsQueryHandler(ICustomerProjectRepository repository, IMapper mapper) : IRequestHandler<GetAllCustomerProjectsQuery, List<CustomerProjectResponseDTO>>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IMapper _mapper = mapper;

		public async Task<List<CustomerProjectResponseDTO>> Handle(GetAllCustomerProjectsQuery request, CancellationToken cancellationToken)
		{
			var projects = await _repository.GetAllAsync(cancellationToken);
			return _mapper.Map<List<CustomerProjectResponseDTO>>(projects);
		}
	}
}