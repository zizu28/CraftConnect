using BookingManagement.Application.DTOs.CraftmanProposalDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries
{
	public class GetProposalsByProjectQuery : IRequest<IEnumerable<CraftsmanProposalResponseDTO>>
	{
		public Guid ProjectId { get; set; }
	}
}
