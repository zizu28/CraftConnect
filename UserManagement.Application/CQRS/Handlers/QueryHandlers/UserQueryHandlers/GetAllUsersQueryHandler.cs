using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
	public class GetAllUsersQueryHandler(
		IMapper mapper, 
		IUserRepository userRepository) : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponseDTO>>
	{
		public async Task<IEnumerable<UserResponseDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
		{
			//var users = await cacheService.GetOrCreateManyAsync<User>("users",
			//	user => user.Id != null, cancellationToken)
			var users = await userRepository.GetAllAsync(cancellationToken)
				?? throw new NotFoundException($"Could not retrieve list of users from database or cache.");

			return mapper.Map<IEnumerable<UserResponseDTO>>(users);
		}
	}
}
