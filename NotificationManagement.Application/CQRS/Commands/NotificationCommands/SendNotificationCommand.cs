using Core.SharedKernel.DTOs;
using MediatR;

namespace NotificationManagement.Application.CQRS.Commands.NotificationCommands;

public class SendNotificationCommand : IRequest<NotificationResponseDTO>
{
	public NotificationCreateDTO? Notification { get; set; }
}
