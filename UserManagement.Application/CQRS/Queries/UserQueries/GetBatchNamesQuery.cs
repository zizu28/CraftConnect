using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetBatchNamesQuery : IRequest<Dictionary<Guid, string>>
	{
		public List<Guid> Ids { get; set; }
	}
}
