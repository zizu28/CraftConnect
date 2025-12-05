using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{

	public class GetUserByIdQueryHandler(
		IMapper mapper, IUserRepository userRepository) 
		: IRequestHandler<GetUserByIdQuery, UserResponseDTO>
	{
		public async Task<UserResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
		{
			var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
				?? throw new NotFoundException($"User with ID {request.UserId} not found in database and cache.");
			return mapper.Map<UserResponseDTO>(user);
		}
	}
}
