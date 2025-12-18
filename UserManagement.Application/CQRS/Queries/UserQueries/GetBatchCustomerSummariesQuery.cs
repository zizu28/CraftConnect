using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetBatchCustomerSummariesQuery : IRequest<Dictionary<Guid, CustomerSummaryDto>>
	{
		public List<Guid> Ids { get; set; } = [];
	}
}
