using BookingManagement.Application.DTOs.CustomerProjectDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetProjectWithDetailsQuery : IRequest<CustomerProjectResponseDTO>
	{
		public Guid ProjectId { get; set; }
	}
}
