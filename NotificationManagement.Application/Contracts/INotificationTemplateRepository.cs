using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.Contracts;

/// <summary>
/// Repository for NotificationTemplate aggregate
/// </summary>
public interface INotificationTemplateRepository
{
	Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
	Task<List<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
	Task<NotificationTemplate> AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
	Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
}
