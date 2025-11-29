using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuditManagement.Application.Extensions
{
	public static class AuditManagementApplicationExtensions
	{
		public static IServiceCollection AddAuditApplicationExtension(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(typeof(AuditManagementApplicationExtensions).Assembly);
				config.LicenseKey = configuration["MediatR:LicenseKey"];
			});
			services.AddAutoMapper(cfg =>
			{
				cfg.AddMaps(typeof(AuditManagementApplicationExtensions).Assembly);
			});
			services.AddValidatorsFromAssemblyContaining(typeof(AuditManagementApplicationExtensions));
			
			return services;
		}
	}
}
