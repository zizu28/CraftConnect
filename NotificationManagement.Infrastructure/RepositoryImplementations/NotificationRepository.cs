using Core.SharedKernel.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Infrastructure.RepositoryImplementations;

public class NotificationRepository(ApplicationDbContext dbContext) 
	: BaseRepository<Notification>(dbContext), INotificationRepository
{
	public async Task<List<Notification>> GetByUserIdAsync(
		Guid userId, 
		int pageNumber, 
		int pageSize, 
		CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.Where(n => n.RecipientId == userId)
			.OrderByDescending(n => n.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<List<Notification>> GetPendingNotificationsAsync(
		CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.Where(n => n.Status == NotificationStatus.Pending || n.Status == NotificationStatus.Scheduled)
			.Where(n => !n.ScheduledFor.HasValue || n.ScheduledFor.Value <= DateTime.UtcNow)
			.OrderBy(n => n.Priority)
			.ThenBy(n => n.CreatedAt)
			.ToListAsync(cancellationToken);
	}
}
