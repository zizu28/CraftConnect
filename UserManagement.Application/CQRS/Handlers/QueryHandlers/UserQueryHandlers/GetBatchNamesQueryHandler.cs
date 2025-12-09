using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.UserQueries;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
	public class GetBatchNamesQueryHandler(
		IUserRepository userRepository) : IRequestHandler<GetBatchNamesQuery, Dictionary<Guid, string>>
	{
		private readonly IUserRepository _userRepository = userRepository;

		public async Task<Dictionary<Guid, string>> Handle(GetBatchNamesQuery request, CancellationToken cancellationToken)
		{
			if (request.Ids == null || !(request.Ids.Count > 0))
			{
				return [];
			}
			var uniqueIds = request.Ids.Distinct().ToList();
			var users = await _userRepository.GetNamesByIdsAsync(uniqueIds, cancellationToken);
			return users;
		}
	}
}
