using Core.SharedKernel.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Infrastructure.RepositoryImplementations;

public class NotificationPreferenceRepository(ApplicationDbContext dbContext) 
	: BaseRepository<NotificationPreference>(dbContext), INotificationPreferenceRepository
{
	public async Task<NotificationPreference?> GetByUserAndTypeAsync(
		Guid userId, 
		NotificationType type, 
		CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationType == type, cancellationToken);
	}

	public async Task<List<NotificationPreference>> GetByUserIdAsync(
		Guid userId, 
		CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.Where(p => p.UserId == userId)
			.OrderBy(p => p.NotificationType)
			.ToListAsync(cancellationToken);
	}
}
