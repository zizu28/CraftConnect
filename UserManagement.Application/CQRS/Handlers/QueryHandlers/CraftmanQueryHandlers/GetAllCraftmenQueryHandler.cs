using AutoMapper;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Handlers.QueryHandlers.Queries.CraftmanQueries;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetAllCraftmenQueryHandler(
		IMapper mapper, ICraftsmanRepository craftsmanRepository)
		: IRequestHandler<GetAllCraftmenQuery, IEnumerable<CraftmanResponseDTO>>
	{
		public async Task<IEnumerable<CraftmanResponseDTO>> Handle(GetAllCraftmenQuery request, CancellationToken cancellationToken)
		{
			var craftmen = await craftsmanRepository.GetAllAsync(cancellationToken)
				?? throw new NotFoundException("Craftmen not found in database or cache.");
			return mapper.Map<IEnumerable<CraftmanResponseDTO>>(craftmen);
		}
	}
}
