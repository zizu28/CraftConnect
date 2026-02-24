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
	public class GetUserByIdQueryHandler(
		IMapper mapper,
		IUserRepository userRepository,
		ICacheService cacheService)
		: IRequestHandler<GetUserByIdQuery, UserResponseDTO>
	{
		public async Task<UserResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
		{
			var user = await cacheService.GetOrCreateAsync<User>(
				CacheKeys.UserById(request.UserId),
				u => u.Id == request.UserId,
				cancellationToken)
				?? throw new NotFoundException($"User with ID {request.UserId} not found in database or cache.");

			return mapper.Map<UserResponseDTO>(user);
		}
	}
}
