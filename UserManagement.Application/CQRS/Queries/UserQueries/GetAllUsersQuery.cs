using MediatR;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetAllUsersQuery : IRequest<IEnumerable<UserResponseDTO>>
	{
	}
}
