using BookingManagement.Application.Contracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingManagement.Infrastructure.RepositoryImplementations
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public Repository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			await _dbSet.AddAsync(entity, cancellationToken);
		}

		public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
		{
			_dbSet.Update(entity);
			await Task.CompletedTask;
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await _dbSet.FindAsync([id], cancellationToken);
			if (entity != null)
			{
				_dbSet.Remove(entity);
			}
		}

		public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _dbSet.FindAsync([id], cancellationToken);
		}

		public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbSet.ToListAsync(cancellationToken);
		}

		public async Task<T?> FindBy(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
		}

		public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await _dbSet.FindAsync([id], cancellationToken);
			return entity != null;
		}
	}
}