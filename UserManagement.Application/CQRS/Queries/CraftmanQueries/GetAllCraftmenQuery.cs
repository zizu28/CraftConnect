using MediatR;
using UserManagement.Application.DTOs.CraftmanDTO;

namespace UserManagement.Application.CQRS.Queries.CraftmanQueries
{
	public class GetAllCraftmenQuery : IRequest<IEnumerable<CraftmanResponseDTO>>
	{
	}
}
