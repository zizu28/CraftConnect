using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetAllCraftmenQueryHandler(
		IMapper mapper,
		ICraftsmanRepository craftsmanRepository)
		: IRequestHandler<GetAllCraftmenQuery, IEnumerable<CraftmanResponseDTO>>
	{
		public async Task<IEnumerable<CraftmanResponseDTO>> Handle(GetAllCraftmenQuery request, CancellationToken cancellationToken)
		{
			var craftmen = await craftsmanRepository.GetAllAsync(cancellationToken)
				?? throw new NotFoundException("Craftmen not found in database.");

			return mapper.Map<IEnumerable<CraftmanResponseDTO>>(craftmen);
		}
	}
}
