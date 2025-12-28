using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationManagement.Application.Extensions;

public static class NotificationManagementApplicationExtensions
{
	public static IServiceCollection AddNotificationApplicationExtensions(
		this IServiceCollection services, 
		IConfiguration configuration)
	{
		// Register MediatR - handles all CQRS commands and queries
		services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssembly(typeof(NotificationManagementApplicationExtensions).Assembly);
			config.LicenseKey = configuration["MediatR:LicenseKey"];
		});

		// Register AutoMapper - handles entity-DTO mapping
		services.AddAutoMapper(cfg =>
		{
			cfg.AddMaps(typeof(NotificationManagementApplicationExtensions).Assembly);
		});

		// Register FluentValidation validators
		services.AddValidatorsFromAssemblyContaining(typeof(NotificationManagementApplicationExtensions));

		// Cross-domain services will be registered here as needed
		// Example: services.AddScoped<INotificationModuleService, NotificationModuleService>();

		return services;
	}
}
