using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.CraftmanQueries
{
	public class GetCraftmanByProfessionQuery : IRequest<CraftmanResponseDTO>
	{
		public Guid CraftmanId { get; set; }
		public string Profession { get; set; } = string.Empty;
	}
}
