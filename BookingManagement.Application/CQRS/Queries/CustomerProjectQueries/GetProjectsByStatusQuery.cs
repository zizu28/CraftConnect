using Core.SharedKernel.DTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class GetProjectsByStatusQuery : IRequest<List<CustomerProjectResponseDTO>>
	{
		public string Status { get; set; } = string.Empty;
	}
}
