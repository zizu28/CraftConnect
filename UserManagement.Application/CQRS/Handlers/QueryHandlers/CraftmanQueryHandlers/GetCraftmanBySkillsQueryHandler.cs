using AutoMapper;
using MassTransit.Initializers;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Handlers.QueryHandlers.Queries.CraftmanQueries;
using UserManagement.Application.DTOs.CraftmanDTO;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers
{
	public class GetCraftmanBySkillsQueryHandler(
		ICraftsmanRepository craftsmanRepository, IMapper mapper)
		: IRequestHandler<GetCraftmanBySkillsQuery, CraftmanResponseDTO>
	{
		public async Task<CraftmanResponseDTO> Handle(GetCraftmanBySkillsQuery request, CancellationToken cancellationToken)
		{
			var craftmanSkills = await craftsmanRepository.GetAllAsync(cancellationToken)
				.Select(craftman => craftman.Where(skilledPerson =>
				{
					return request.Skills.All(skill => skilledPerson
						.Skills.Any(s => s.Name.Equals(skill.Name) && s.YearsOfExperience >= skill.YearsOfExperience));
				}))
				?? throw new NotFoundException($"Craftmen with skills {request.Skills.Select(s => s.Name).ToList()} not found.");

			var craftman = craftmanSkills.FirstOrDefault(c => c.Id == request.CraftmanId)
				?? throw new NotFoundException($"Craftman with ID {request.CraftmanId} not found.");
			return mapper.Map<CraftmanResponseDTO>(craftman);
		}
	}
}
