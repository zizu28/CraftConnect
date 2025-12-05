using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.CraftmanQueries
{
	public class GetCraftmanByIdQuery : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
	}
}
