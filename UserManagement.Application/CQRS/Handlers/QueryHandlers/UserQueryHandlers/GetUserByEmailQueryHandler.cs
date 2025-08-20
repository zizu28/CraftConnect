using AutoMapper;
using Core.SharedKernel.ValueObjects;
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
			var email = new Email(request.Email);
			var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
				?? throw new NotFoundException($"User with email {request.Email} not found.");
			return mapper.Map<UserResponseDTO>(user);
		}
	}
}
