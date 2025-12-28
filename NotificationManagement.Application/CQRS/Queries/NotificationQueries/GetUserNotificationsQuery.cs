using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Queries.NotificationQueries;

public class GetUserNotificationsQuery : IRequest<List<NotificationResponseDTO>>
{
	public Guid UserId { get; set; }
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 20;
}
