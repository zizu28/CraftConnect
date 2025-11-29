using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.Queries.CraftmanQueries
{
	public class GetCraftmanByIdQuery : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
	}
}
