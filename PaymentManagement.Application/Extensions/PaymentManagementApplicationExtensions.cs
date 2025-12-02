using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Retry;
using System.Threading.RateLimiting;

namespace PaymentManagement.Application.Extensions
{
	public static class PaymentManagementApplicationExtensions
	{
		public static IServiceCollection AddPaymentManagementApplicationConfigurations(
			this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(typeof(PaymentManagementApplicationExtensions).Assembly);
				config.LicenseKey = configuration["MediatR:LicenseKey"];
			});
			services.AddAutoMapper(cfg =>
			{
				cfg.AddMaps(typeof(PaymentManagementApplicationExtensions).Assembly);
			});
			services.AddValidatorsFromAssemblyContaining(typeof(PaymentManagementApplicationExtensions));

			services.AddHttpClient("PaystackClient", client =>
			{
				client.BaseAddress = new Uri("https://api.paystack.co/");
				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Paystack:SecretKey"]}");
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			});
			//.AddResilienceHandler("", pipeline =>
			//{
			//	pipeline.AddRetry(new HttpRetryStrategyOptions
			//	{
			//		MaxRetryAttempts = 3,
			//		Delay = TimeSpan.FromMilliseconds(500),
			//		BackoffType = DelayBackoffType.Exponential,
			//		UseJitter = true
			//	});
			//	pipeline.AddTimeout(TimeSpan.FromSeconds(30));
			//	pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
			//	{
			//		MinimumThroughput = 8,
			//		SamplingDuration = TimeSpan.FromSeconds(30),
			//		FailureRatio = 0.9,
			//		BreakDuration = TimeSpan.FromSeconds(5),
			//	});
			//	pipeline.AddRateLimiter(new HttpRateLimiterStrategyOptions
			//	{
			//		DefaultRateLimiterOptions = new ConcurrencyLimiterOptions
			//		{
			//			PermitLimit = 100,
			//			QueueLimit = 0,
			//			QueueProcessingOrder = QueueProcessingOrder.OldestFirst
			//		}
			//	});
			//});

			services.AddCors(services =>
			{
				services.AddPolicy("PaystackCors", builder =>
				{
					builder.AllowAnyOrigin()
						   .AllowAnyMethod()
						   .AllowAnyHeader();
				});
			});
			return services;
		}
	}
}
