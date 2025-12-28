using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Queries.NotificationQueries;

public class GetNotificationByIdQuery : IRequest<NotificationResponseDTO?>
{
	public Guid NotificationId { get; set; }
}
