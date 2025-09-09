using System.Linq.Expressions;

namespace BookingManagement.Application.Contracts
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<T?> FindBy(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
		Task AddAsync(T entity, CancellationToken cancellationToken = default);
		Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
		Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
