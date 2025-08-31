using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventServices
{
	public static class MassTransitMessageExtensions
	{
		public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
		{
			//var rabbitMQConfig = configuration.GetSection("RabbitMQ");
			//if (rabbitMQConfig == null || !rabbitMQConfig.Exists())
			//{
			//	throw new ArgumentException("RabbitMQ configuration section is missing or invalid.");
			//}
				services.AddMassTransit(mt =>
			{
				//mt.UsingRabbitMq((context, config) =>
				//{
				//	config.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
				//	{
				//		h.Username(configuration["RabbitMQ:UserName"]!);
				//		h.Password(configuration["RabbitMQ:Password"]!);
				//	});
				//	config.UseMessageRetry(retry =>
				//	{
				//		retry.Exponential(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(5));
				//	});
				//	config.ConfigureEndpoints(context);
				//});

				mt.UsingAmazonSqs((context, config) =>
				{
					config.Host("us-east-1", h =>
					{
						h.AccessKey(configuration["AWS:AccessKey"]!);
						h.SecretKey(configuration["AWS:SecretKey"]!);
						h.Scope("CraftConnect", true);
					});
					config.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("dev", false));
				});
			});

			services.AddScoped<IMessageBroker, MassTransitMessageBroker>();
			return services;
		}
	}
}
