using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.UserQueries;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
	public class CheckUserExistsQueryHandler(
		IUserRepository userRepository) : IRequestHandler<CheckUserExistsQuery, bool>
	{
		private readonly IUserRepository _userRepository = userRepository;

		public async Task<bool> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
		{
			return await _userRepository.ExistsAsync(request.UserId, cancellationToken);
		}
	}
}
