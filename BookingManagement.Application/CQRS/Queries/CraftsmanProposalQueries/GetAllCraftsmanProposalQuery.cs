using BookingManagement.Application.DTOs.CraftmanProposalDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries
{
	public class GetAllCraftsmanProposalQuery : IRequest<IEnumerable<CraftsmanProposalResponseDTO>>
	{
	}
}
