using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetUserByEmailQuery : IRequest<UserResponseDTO>
	{
		public required string Email { get; set; }
	}
}
