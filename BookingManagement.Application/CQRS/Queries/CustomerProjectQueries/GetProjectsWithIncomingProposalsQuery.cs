using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetProjectsWithIncomingProposalsQuery : IRequest<List<CraftsmanProposalResponseDTO>>
	{
		public Guid CustomerId { get; set; }
	}
}
