using Core.SharedKernel.DTOs;
using MediatR;

namespace UserManagement.Application.CQRS.Queries.UserQueries
{
	public class GetUserByIdQuery : IRequest<UserResponseDTO>
	{
		public Guid UserId { get; set; }
	}
}
