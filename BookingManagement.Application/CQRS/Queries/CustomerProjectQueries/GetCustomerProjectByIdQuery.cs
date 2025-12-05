using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetCustomerProjectByIdQuery : IRequest<CustomerProjectResponseDTO>
	{
		public Guid ProjectId { get; set; }
	}
}
