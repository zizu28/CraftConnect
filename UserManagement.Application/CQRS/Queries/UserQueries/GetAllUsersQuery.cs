using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetAllUsersQuery : IRequest<IEnumerable<UserResponseDTO>>
	{
	}
}
