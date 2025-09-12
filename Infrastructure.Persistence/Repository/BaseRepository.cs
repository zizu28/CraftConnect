using Core.SharedKernel.Contracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repository
{
	public abstract class BaseRepository<T> : IRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _dbContext;
		protected readonly DbSet<T> _dbSet;

		protected BaseRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_dbSet = _dbContext.Set<T>();
		}

		public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			await _dbSet.AddAsync(entity, cancellationToken);
			return entity;
		}

		public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await _dbSet.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"{typeof(T).Name} with ID {id} not found.");
			_dbSet.Remove(entity);
		}

		public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _dbSet.FindAsync([id], cancellationToken) != null;
		}

		public virtual async Task<T?> FindBy(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
		}

		public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
		}

		public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
		}

		public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			_dbContext.Entry(entity).State = EntityState.Modified;
			return Task.CompletedTask;
		}
	}
}