using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.CraftmanQueries
{
	public class GetAllCraftmenQuery : IRequest<IEnumerable<CraftmanResponseDTO>>
	{
	}
}
