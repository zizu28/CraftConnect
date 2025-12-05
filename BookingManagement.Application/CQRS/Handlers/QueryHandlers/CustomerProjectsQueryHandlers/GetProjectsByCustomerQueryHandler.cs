using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Handlers.QueryHandlers.CustomerProjectsQueryHandlers
{
	public class GetProjectsByCustomerQueryHandler(ICustomerProjectRepository repository, IMapper mapper) : IRequestHandler<GetProjectsByCustomerQuery, List<CustomerProjectResponseDTO>>
	{
		private readonly ICustomerProjectRepository _repository = repository;
		private readonly IMapper _mapper = mapper;

		public async Task<List<CustomerProjectResponseDTO>> Handle(GetProjectsByCustomerQuery request, CancellationToken cancellationToken)
		{
			var projects = await _repository.GetProjectsByCustomerAsync(request.CustomerId, cancellationToken);
			return _mapper.Map<List<CustomerProjectResponseDTO>>(projects);
		}
	}
}