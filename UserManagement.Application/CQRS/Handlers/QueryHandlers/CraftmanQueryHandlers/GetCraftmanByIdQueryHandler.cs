using AutoMapper;
using Core.SharedKernel.DTOs;
using Infrastructure.Cache;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetCraftmanByIdQueryHandler(
		IMapper mapper,
		ICraftsmanRepository craftsmanRepository,
		ICacheService cacheService)
		: IRequestHandler<GetCraftmanByIdQuery, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(GetCraftmanByIdQuery request, CancellationToken cancellationToken)
		{
			var craftman = await cacheService.GetOrCreateAsync<Craftman>(
				CacheKeys.CraftsmanById(request.CraftmanId),
				c => c.Id == request.CraftmanId,
				cancellationToken)
				?? throw new NotFoundException($"Craftman with ID {request.CraftmanId} not found.");

			return mapper.Map<CraftmanResponseDTO>(craftman);
		}
	}
}
