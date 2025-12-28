using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Commands.NotificationCommands;

namespace NotificationManagement.Application.CQRS.Handlers.CommandHandlers.NotificationCommandHandlers;

public class CancelNotificationCommandHandler(
	ILoggingService<CancelNotificationCommandHandler> logger,
	INotificationRepository notificationRepository,
	IUnitOfWork unitOfWork) : IRequestHandler<CancelNotificationCommand, Unit>
{
	public async Task<Unit> Handle(CancelNotificationCommand request, CancellationToken cancellationToken)
	{
		var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken) ?? throw new KeyNotFoundException($"Notification {request.NotificationId} not found");
		notification.Cancel(request.Reason);

		await unitOfWork.ExecuteInTransactionAsync(async () =>
		{
			await notificationRepository.UpdateAsync(notification, cancellationToken);
		}, cancellationToken);

		logger.LogInformation("Notification {NotificationId} cancelled. Reason: {Reason}", request.NotificationId, request.Reason);

		return Unit.Value;
	}
}
