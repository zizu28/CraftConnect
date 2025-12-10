using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetBatchCraftsmanSummariesQuery : IRequest<Dictionary<Guid, CraftsmanSummaryDto>>
	{
		public List<Guid> Ids { get; set; } = [];
	}
}
