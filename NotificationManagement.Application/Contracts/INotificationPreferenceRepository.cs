using Core.SharedKernel.Enums;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.Contracts;

/// <summary>
/// Repository for NotificationPreference aggregate
/// </summary>
public interface INotificationPreferenceRepository
{
	Task<NotificationPreference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<NotificationPreference?> GetByUserAndTypeAsync(Guid userId, NotificationType type, CancellationToken cancellationToken = default);
	Task<List<NotificationPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
	Task<NotificationPreference> AddAsync(NotificationPreference preference, CancellationToken cancellationToken = default);
	Task UpdateAsync(NotificationPreference preference, CancellationToken cancellationToken = default);
}
