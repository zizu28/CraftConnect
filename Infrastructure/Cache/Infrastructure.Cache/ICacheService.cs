using System.Linq.Expressions;

namespace Infrastructure.Cache
{
	public interface ICacheService
	{
		Task<T?> GetOrCreateAsync<T>(string key, Expression<Func<T, bool>> predicate, CancellationToken token) where T : class;
		Task SetAsync<T>(string key, T value, CancellationToken token) where T : class;

		Task<IEnumerable<T>?> GetOrCreateManyAsync<T>(string key, Expression<Func<T, bool>> predicate, CancellationToken token)where T : class;
		Task SetManyAsync<T>(string key, IEnumerable<T> values, CancellationToken token) where T : class;
		Task RemoveSync(string key, CancellationToken token);
	}
}
