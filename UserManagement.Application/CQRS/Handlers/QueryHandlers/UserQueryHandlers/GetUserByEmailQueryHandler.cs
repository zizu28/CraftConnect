using AutoMapper;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.Exceptions;

namespace UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers
{
	public class GetUserByEmailQueryHandler(
		IMapper mapper, IUserRepository userRepository) 
		: IRequestHandler<GetUserByEmailQuery, UserResponseDTO>
	{
		public async Task<UserResponseDTO> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
		{
			var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
				?? throw new NotFoundException($"User with email {request.Email} not found.");
			var User = mapper.Map<UserResponseDTO>(existingUser);
			return User;
		}
	}
}
