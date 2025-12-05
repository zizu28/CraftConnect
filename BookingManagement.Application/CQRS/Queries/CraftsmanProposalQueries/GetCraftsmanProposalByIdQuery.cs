using BookingManagement.Application.DTOs.CraftmanProposalDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries
{
	public class GetCraftsmanProposalByIdQuery : IRequest<CraftsmanProposalResponseDTO?>
	{
		public Guid ProposalId { get; set; }
	}
}
