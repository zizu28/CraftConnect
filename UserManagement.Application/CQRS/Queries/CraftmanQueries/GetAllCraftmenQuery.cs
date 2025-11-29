using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.Queries.CraftmanQueries
{
	public class GetAllCraftmenQuery : IRequest<IEnumerable<CraftmanResponseDTO>>
	{
	}
}
