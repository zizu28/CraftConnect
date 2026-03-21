using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;

namespace CraftConnect.ServiceDefaults
{
	// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
	// This project should be referenced by each service project in your solution.
	// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
	public static class Extensions
	{
		private const string HealthEndpointPath = "/health";
		private const string AlivenessEndpointPath = "/alive";

		public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
		{
			builder.ConfigureOpenTelemetry();

			builder.AddDefaultHealthChecks();

			builder.Services.AddServiceDiscovery();

			builder.Services.ConfigureHttpClientDefaults(http =>
			{
				// Turn on resilience by default
				http.AddStandardResilienceHandler(opt =>
				{
					// Only retry on genuine transient failures — NOT on 2xx or 429
					opt.Retry.ShouldHandle = args => args.Outcome switch
					{
						// Exceptions (network failures, timeouts) — always retry
						{ Exception: not null } => PredicateResult.True(),
						// Transient server errors: 5xx and 408 Request Timeout
						{ Result: HttpResponseMessage r }
							when (int)r.StatusCode >= 500
							  || r.StatusCode == System.Net.HttpStatusCode.RequestTimeout
							=> PredicateResult.True(),
						// Everything else (200 OK, 401, 429, etc.) — do NOT retry
						_ => PredicateResult.False()
					};
#if DEBUG
					opt.AttemptTimeout.Timeout = TimeSpan.FromMinutes(5);
					opt.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(10);
					// SamplingDuration must be >= 2x AttemptTimeout (5min * 2 = 10min)
					opt.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(11);
					// Disable retries — re-execution behind a breakpoint is confusing
					opt.Retry.MaxRetryAttempts = 3;
#else
					// Sensible production values
					opt.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
					opt.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
#endif
				});

				// Turn on service discovery by default
				http.AddServiceDiscovery();
			});

			// Uncomment the following to restrict the allowed schemes for service discovery.
			//builder.Services.Configure<ServiceDiscoveryOptions>(options =>
			//{
			//	options.AllowedSchemes = ["https"];
			//});

			return builder;
		}

		public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
		{
			builder.Logging.AddOpenTelemetry(logging =>
			{
				logging.IncludeFormattedMessage = true;
				logging.IncludeScopes = true;
			});

			builder.Services.AddOpenTelemetry()
				.WithMetrics(metrics =>
				{
					metrics.AddAspNetCoreInstrumentation()
						.AddHttpClientInstrumentation()
						.AddRuntimeInstrumentation();
				})
				.WithTracing(tracing =>
				{
					tracing.AddSource(builder.Environment.ApplicationName)
						.AddAspNetCoreInstrumentation(tracing =>
							// Exclude health check requests from tracing
							tracing.Filter = context =>
								!context.Request.Path.StartsWithSegments(HealthEndpointPath)
								&& !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
						)
						// Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
						//.AddGrpcClientInstrumentation()
						.AddHttpClientInstrumentation();
				});

			builder.AddOpenTelemetryExporters();

			return builder;
		}

		private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
		{
			var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

			if (useOtlpExporter)
			{
				builder.Services.AddOpenTelemetry().UseOtlpExporter();
			}

			// Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
			//if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
			//{
			//    builder.Services.AddOpenTelemetry()
			//       .UseAzureMonitor();
			//}

			return builder;
		}

		public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
		{
			builder.Services.AddHealthChecks()
				// Add a default liveness check to ensure app is responsive
				.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

			return builder;
		}

		public static WebApplication MapDefaultEndpoints(this WebApplication app)
		{
			// Adding health checks endpoints to applications in non-development environments has security implications.
			// See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
			if (app.Environment.IsDevelopment())
			{
				// All health checks must pass for app to be considered ready to accept traffic after starting
				app.MapHealthChecks(HealthEndpointPath);

				// Only health checks tagged with the "live" tag must pass for app to be considered alive
				app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
				{
					Predicate = r => r.Tags.Contains("live")
				});
			}

			return app;
		}
	}
}
