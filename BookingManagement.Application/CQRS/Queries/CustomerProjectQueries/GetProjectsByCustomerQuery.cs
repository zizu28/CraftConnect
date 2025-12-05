using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetProjectsByCustomerQuery : IRequest<List<CustomerProjectResponseDTO>>
	{
		public Guid CustomerId { get; set; }
	}
}
