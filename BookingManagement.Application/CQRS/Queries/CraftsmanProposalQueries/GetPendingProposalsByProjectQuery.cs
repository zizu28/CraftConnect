using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries
{
	public class GetPendingProposalsByProjectQuery : IRequest<IEnumerable<CraftsmanProposalResponseDTO>>
	{
		public Guid ProjectId { get; set; }
	}
}
