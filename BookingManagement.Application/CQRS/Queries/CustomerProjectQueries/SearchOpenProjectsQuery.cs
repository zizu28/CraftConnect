using BookingManagement.Application.DTOs.CustomerProjectDTOs;
using MediatR;

namespace BookingManagement.Application.CQRS.Queries.CustomerProjectQueries
{
	public class SearchOpenProjectsQuery : IRequest<List<CustomerProjectResponseDTO>>
	{
		public string? SearchTerm { get; set; }
		public List<string>? Skills { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
