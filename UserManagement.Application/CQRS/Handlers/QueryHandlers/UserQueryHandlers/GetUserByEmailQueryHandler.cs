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
	public class GetUserByEmailQueryHandler(
		IMapper mapper,
		IUserRepository userRepository,
		ICacheService cacheService)
		: IRequestHandler<GetUserByEmailQuery, UserResponseDTO>
	{
		public async Task<UserResponseDTO> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
		{
			var existingUser = await cacheService.GetOrCreateAsync<User>(
				CacheKeys.UserByEmail(request.Email),
				u => u.Email.Address == request.Email,
				cancellationToken)
				?? throw new NotFoundException($"User with email {request.Email} not found.");

			return mapper.Map<UserResponseDTO>(existingUser);
		}
	}
}
