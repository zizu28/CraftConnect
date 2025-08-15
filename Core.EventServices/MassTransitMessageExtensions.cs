using Infrastructure.Persistence.Data;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.EventServices
{
	public static class MassTransitMessageExtensions
	{
		public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMassTransit(mt =>
			{
				mt.UsingRabbitMq((context, config) =>
				{
					//config.ReceiveEndpoint("event-bus", e =>
					//{
					//	e.Durable = true;
					//	e.AutoDelete = false;
					//	e.Exclusive = false;
					//});
					config.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
					{
						h.Username(configuration["RabbitMQ:UserName"]!);
						h.Password(configuration["RabbitMQ:Password"]!);
					});
					config.UseMessageRetry(retry =>
					{
						retry.Exponential(10, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(5));
					});
					config.ConfigureEndpoints(context);
				});
			});

			services.AddScoped<IMessageBroker, MassTransitMessageBroker>();
			services.AddScoped<IMassTransitIntegrationEventBus, MassTransitIntegrationEventBus>();
			return services;
		}
	}
}
