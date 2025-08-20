using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cache
{
	public static class HybridCacheExtension
	{
		public static IServiceCollection AddHybridCacheService(this IServiceCollection services, IConfiguration config)
		{
			services.AddStackExchangeRedisCache(opt =>
			{
				opt.Configuration = config.GetConnectionString("Redis");
				opt.InstanceName = "Users:";
			});
			services.AddHybridCache(opt =>
			{
				opt.MaximumKeyLength = 512;
			});
			services.AddScoped<ICacheService, HybridCacheService>();
			services.AddSingleton(sp =>
			{
				var redisConnection = config.GetConnectionString("Redis");
				return StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnection!);
			});
			return services;
		}
	}
}
