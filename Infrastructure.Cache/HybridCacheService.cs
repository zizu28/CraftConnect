using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using System.Linq.Expressions;

namespace Infrastructure.Cache
{
	public class HybridCacheService(
		HybridCache cache, ApplicationDbContext dbContext) : ICacheService
	{
		private readonly HybridCacheEntryOptions defaultCacheOptions = new HybridCacheEntryOptions
		{
			LocalCacheExpiration = TimeSpan.FromMinutes(5),
			Expiration = TimeSpan.FromMinutes(10)
		};
		public async Task<T?> GetOrCreateAsync<T>(string key, Expression<Func<T, bool>> predicate, CancellationToken token) where T : class
		{
			return await cache.GetOrCreateAsync(
				key,
				async entry => await dbContext.Set<T>().FirstOrDefaultAsync(predicate, entry),
				defaultCacheOptions, cancellationToken: token);
		}

		public async Task<IEnumerable<T>?> GetOrCreateManyAsync<T>(string key, Expression<Func<T, bool>> predicate, CancellationToken token) where T : class
		{
			return await cache.GetOrCreateAsync(
				key,
				async entry =>
				{
					var query = dbContext.Set<T>().AsNoTracking();
					if(predicate is not null)
					{
						query = query.Where(predicate);
					}
					return await query.ToListAsync(entry);
				},
				defaultCacheOptions, cancellationToken: token);
		}

		public async Task RemoveSync(string key, CancellationToken token)
		{
			await cache.RemoveAsync(key, token);
		}

		public async Task SetAsync<T>(string key, T value, CancellationToken token) where T : class
		{
			await cache.SetAsync(key, value, defaultCacheOptions, cancellationToken: token);
		}

		public async Task SetManyAsync<T>(string key, IEnumerable<T> values, CancellationToken token) where T : class
		{
			await cache.SetAsync(key, values, defaultCacheOptions, cancellationToken: token);
		}
	}
}
