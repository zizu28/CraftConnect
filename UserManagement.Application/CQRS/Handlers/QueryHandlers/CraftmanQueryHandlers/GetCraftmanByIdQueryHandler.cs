using AutoMapper;
using Infrastructure.Cache;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Handlers.QueryHandlers.Queries.CraftmanQueries;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetCraftmanByIdQueryHandler(
		IMapper mapper, ICraftsmanRepository craftsmanRepository)
		: IRequestHandler<GetCraftmanByIdQuery, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(GetCraftmanByIdQuery request, CancellationToken cancellationToken)
		{
			var craftman = await craftsmanRepository.GetByIdAsync(request.CraftmanId, cancellationToken)
				?? throw new NotFoundException($"Craftman with ID {request.CraftmanId} not found.");
			return mapper.Map<CraftmanResponseDTO>(craftman);
		}
	}
}
