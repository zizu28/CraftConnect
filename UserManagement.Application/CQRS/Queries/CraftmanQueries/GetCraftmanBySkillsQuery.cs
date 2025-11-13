using Core.SharedKernel.ValueObjects;
using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.Queries.CraftmanQueries
{
	public class GetCraftmanBySkillsQuery : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
		public List<Skill> Skills { get; set; } = [];
	}
}
