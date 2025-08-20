using MediatR;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetUserByEmailQuery : IRequest<UserResponseDTO>
	{
		public required string Email { get; set; }
	}
}
