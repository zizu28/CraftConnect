using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class CheckUserExistsQuery : IRequest<bool>
	{
		public Guid UserId { get; set; }
	}
}
