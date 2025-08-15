//using Infrastructure.Persistence.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Hybrid;
//using System.Linq.Expressions;
//using UserManagement.Domain.Entities;

//namespace Infrastructure.Cache.UsersCache
//{
//	public class UsersCacheService(
//		HybridCache cache,
//		ApplicationDbContext dbContext) : IUsersCacheService
//	{
//		private readonly HybridCacheEntryOptions cacheOptions = new HybridCacheEntryOptions
//		{
//			LocalCacheExpiration = TimeSpan.FromMinutes(5),
//			Expiration = TimeSpan.FromMinutes(10)
//		};

//		public async Task<User?> GetOrCreateAsync(string key, Expression<Func<User, bool>> predicate, CancellationToken token)
//		{
//			return await cache.GetOrCreateAsync(key,
//				async entry => await dbContext.Users.FirstOrDefaultAsync(predicate, entry),
//				cacheOptions,
//				cancellationToken: token);
//		}

//		public async Task<IEnumerable<User>?> GetOrCreateManyAsync(string key, Expression<Func<User, bool>> predicate, CancellationToken token)
//		{
//			return await cache.GetOrCreateAsync(key,
//				async entry => await dbContext.Users.AsNoTracking().ToListAsync(entry),
//				cacheOptions,
//				cancellationToken: token);
//		}

//		public async Task RemoveSync(string key, CancellationToken token)
//		{
//			await cache.RemoveAsync(key, token);
//		}

//		public async Task SetAsync(string key, User value, CancellationToken token)
//		{
//			await cache.SetAsync(key, value, cacheOptions, cancellationToken: token);
//		}

//		public async Task SetManyAsync(string key, IEnumerable<User> values, CancellationToken token)
//		{
//			if (values is null || !values.Any())
//			{
//				throw new ArgumentException("Values cannot be null or empty.", nameof(values));
//			}
//			await cache.SetAsync(key, values, cacheOptions, cancellationToken: token);
//		}
//	}
//}
