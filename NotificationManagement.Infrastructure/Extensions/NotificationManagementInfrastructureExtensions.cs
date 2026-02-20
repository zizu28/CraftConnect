using Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Infrastructure.Providers;
using NotificationManagement.Infrastructure.RepositoryImplementations;

namespace NotificationManagement.Infrastructure.Extensions;

public static class NotificationManagementInfrastructureExtensions
{
	public static IServiceCollection AddNotificationInfrastructureExtensions(
		this IServiceCollection services, IConfiguration configuration)
	{
		// Register UnitOfWork
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		// Register Repositories
		services.AddScoped<INotificationRepository, NotificationRepository>();
		services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
		services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

		// Register Notification Provider
		services.AddScoped<INotificationProvider, EmailNotificationProvider>();

		return services;
	}
}
