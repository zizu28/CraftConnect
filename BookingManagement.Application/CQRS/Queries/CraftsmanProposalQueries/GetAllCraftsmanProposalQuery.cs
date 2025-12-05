using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries
{
	public class GetAllCraftsmanProposalQuery : IRequest<IEnumerable<CraftsmanProposalResponseDTO>>
	{
	}
}
