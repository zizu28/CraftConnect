using AutoMapper;
using Core.SharedKernel.DTOs;
using Infrastructure.Cache;
using MassTransit.Initializers;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetCraftmanBySkillsQueryHandler(
		ICraftsmanRepository craftsmanRepository,
		IMapper mapper,
		ICacheService cacheService)
		: IRequestHandler<GetCraftmanBySkillsQuery, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(GetCraftmanBySkillsQuery request, CancellationToken cancellationToken)
		{
			// Re-use the shared AllCraftsmen cache entry and filter in-memory,
			// the same pattern as GetCraftmanByProfession.
			var allCraftsmen = await cacheService.GetOrCreateManyAsync<Craftman>(
				CacheKeys.AllCraftsmen,
				c => true,
				cancellationToken)
				?? throw new NotFoundException($"Craftmen with skills {string.Join(", ", request.Skills.Select(s => s.Name))} not found.");

			var craftman = allCraftsmen
				.Where(c => request.Skills.All(skill =>
					c.Skills.Any(s => s.Name.Equals(skill.Name) && s.YearsOfExperience >= skill.YearsOfExperience)))
				.FirstOrDefault(c => c.Id == request.CraftmanId)
				?? throw new NotFoundException($"Craftman with ID {request.CraftmanId} not found.");

			return mapper.Map<CraftmanResponseDTO>(craftman);
		}
	}
}
