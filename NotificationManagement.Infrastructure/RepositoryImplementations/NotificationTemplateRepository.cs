using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Infrastructure.RepositoryImplementations;

public class NotificationTemplateRepository(ApplicationDbContext dbContext) 
	: BaseRepository<NotificationTemplate>(dbContext), INotificationTemplateRepository
{
	public async Task<NotificationTemplate?> GetByCodeAsync(
		string code, 
		CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
	}

	public async Task<List<NotificationTemplate>> GetActiveTemplatesAsync(
		CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.Where(t => t.IsActive)
			.OrderBy(t => t.Name)
			.ToListAsync(cancellationToken);
	}
}
