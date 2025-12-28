using MediatR;

namespace NotificationManagement.Application.CQRS.Commands.NotificationCommands;

public class CancelNotificationCommand : IRequest<Unit>
{
	public Guid NotificationId { get; set; }
	public string Reason { get; set; } = string.Empty;
}
