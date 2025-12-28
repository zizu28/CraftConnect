using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.Contracts;

/// <summary>
/// Repository for Notification aggregate
/// </summary>
public interface INotificationRepository
{
	Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<List<Notification>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
	Task<List<Notification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
	Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);
	Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
