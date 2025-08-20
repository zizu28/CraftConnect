using MediatR;
using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetUserByIdQuery : IRequest<UserResponseDTO>
	{
		public Guid UserId { get; set; }
	}
}
