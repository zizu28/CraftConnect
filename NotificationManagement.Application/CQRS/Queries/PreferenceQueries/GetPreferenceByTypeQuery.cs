using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using MediatR;

namespace NotificationManagement.Application.CQRS.Queries.PreferenceQueries;

public class GetPreferenceByTypeQuery : IRequest<NotificationPreferenceResponseDTO?>
{
	public Guid UserId { get; set; }
	public NotificationType NotificationType { get; set; }
}
