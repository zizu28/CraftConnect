using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetProjectWithDetailsQuery : IRequest<CustomerProjectResponseDTO>
	{
		public Guid ProjectId { get; set; }
	}
}
