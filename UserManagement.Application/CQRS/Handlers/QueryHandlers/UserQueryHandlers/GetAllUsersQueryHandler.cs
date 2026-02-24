using AutoMapper;
using Core.SharedKernel.DTOs;
using Infrastructure.Cache;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.Exceptions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
	public class GetAllUsersQueryHandler(
		IMapper mapper,
		IUserRepository userRepository,
		ICacheService cacheService) : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponseDTO>>
	{
		public async Task<IEnumerable<UserResponseDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
		{
			var users = await cacheService.GetOrCreateManyAsync<User>(
				CacheKeys.AllUsers,
				null,
				cancellationToken)
				?? throw new NotFoundException("Could not retrieve list of users from database or cache.");

			return mapper.Map<IEnumerable<UserResponseDTO>>(users);
		}
	}
}
