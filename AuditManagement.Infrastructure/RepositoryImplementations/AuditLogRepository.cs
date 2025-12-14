using AuditManagement.Application.Contracts;
using AuditManagement.Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuditManagement.Infrastructure.RepositoryImplementations
{
	public class AuditLogRepository(ApplicationDbContext dbContext) : IAuditLogRepository
	{
		public async Task AddAsync(AuditLog entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity, nameof(entity));
			await dbContext.AuditLogs.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var auditLog = await dbContext.AuditLogs.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"Audit log with Id {id} not found.");
			dbContext.AuditLogs.Remove(auditLog);
		}

		public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return (await FindBy(au => au.Id == id, cancellationToken) != null);
		}

		public async Task<AuditLog?> FindBy(Expression<Func<AuditLog, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			var auditLog = await dbContext.AuditLogs.FirstOrDefaultAsync(predicate, cancellationToken)
				?? throw new KeyNotFoundException($"Audit log not found");
			return auditLog;
		}

		public async Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return (await dbContext.AuditLogs
				.AsNoTracking()
				.ToListAsync(cancellationToken)
			);
		}

		public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var auditLog = await FindBy(au => au.Id == id, cancellationToken);
			return auditLog;
		}

		public Task UpdateAsync(AuditLog entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			dbContext.AuditLogs.Update(entity);
			return Task.CompletedTask;
		}
	}
}
