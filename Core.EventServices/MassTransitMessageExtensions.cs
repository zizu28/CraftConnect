using Infrastructure.Persistence.Data;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventServices
{
	public static class MassTransitMessageExtensions
	{
		public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMassTransit(mt =>
			{
				mt.AddEntityFrameworkOutbox<ApplicationDbContext>(config =>
				{
					config.QueryDelay = TimeSpan.FromSeconds(30);
					config.UseSqlServer().UseBusOutbox();
				});
				mt.SetKebabCaseEndpointNameFormatter();
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


