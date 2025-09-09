using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.UnitOfWork
{
	public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
	{
		private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
		private IDbContextTransaction? _transaction;

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}

		public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			_transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
			return _transaction;
		}

		public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction != null)
			{
				await _transaction.CommitAsync(cancellationToken);
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync(cancellationToken);
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
		{
			using var transaction = await BeginTransactionAsync(cancellationToken);
			try
			{
				await operation();
				await SaveChangesAsync(cancellationToken);
				await CommitTransactionAsync(cancellationToken);
			}
			catch
			{
				await RollbackTransactionAsync(cancellationToken);
				throw;
			}
		}

		public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
		{
			using var transaction = await BeginTransactionAsync(cancellationToken);
			try
			{
				var result = await operation();
				await SaveChangesAsync(cancellationToken);
				await CommitTransactionAsync(cancellationToken);
				return result;
			}
			catch
			{
				await RollbackTransactionAsync(cancellationToken);
				throw;
			}
		}

		public void Dispose()
		{
			_transaction?.Dispose();
			_context.Dispose();
		}
	}
}