using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetProposalsByProjectQuery : IRequest<IEnumerable<CraftsmanProposalResponseDTO>>
	{
		public Guid ProjectId { get; set; }
	}
}
