using BookingManagement.Application.DTOs.CraftmanProposalDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries
{
	public class GetProposalsByCraftsmanQuery : IRequest<IEnumerable<CraftsmanProposalResponseDTO>>
	{
		public Guid CraftsmanId { get; set; }
	}
}
