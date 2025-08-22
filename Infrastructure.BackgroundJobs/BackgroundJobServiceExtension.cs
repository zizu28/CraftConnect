using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.BackgroundJobs
{
	public static class BackgroundJobServiceExtension
	{
		public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration config)
		{
			services.AddHangfire(configuration =>
			{
				configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
				configuration.UseSimpleAssemblyNameTypeSerializer();
				configuration.UseRecommendedSerializerSettings();
				configuration.UsePostgreSqlStorage(p =>
				{
					p.UseNpgsqlConnection(config.GetConnectionString("HangfireConnection"));
				}); 
			});

			services.AddHangfireServer(options =>
			{
				options.WorkerCount = Environment.ProcessorCount * 2;
				options.Queues = ["alpha", "beta", "default"];
			});

			//services.AddHangfireOutboxService();

			services.AddScoped<IBackgroundJobService, HangfireJobService>();
			services.AddScoped<QueuedEmailService>();

			return services;
		}

		public static IApplicationBuilder AddHangfireDashBoard(this IApplicationBuilder app)
		{
			//app.UseHangfireDashboard("/hangfire", new DashboardOptions
			//{
			//	Authorization = [new HangfireAuthFilter()]
			//});
			app.UseHangfireDashboard();

			return app;
		}
	}
}
