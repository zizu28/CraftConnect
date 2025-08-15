using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Core.Logging
{
	public static class SerilogLoggerExtension
	{
		public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
		{
			builder.UseSerilog((context, services, configuration) =>
			{
				configuration
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
				.WriteTo.Console()
				.WriteTo.File("logs/log-.txt",
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: null);
			});
			return builder;
		}

		public static IServiceCollection RegisterSerilog(this IServiceCollection services)
		{
			services.AddScoped(typeof(ILoggingService<>), typeof(SerilogLoggerService<>));
			return services;
		}
	}
}
