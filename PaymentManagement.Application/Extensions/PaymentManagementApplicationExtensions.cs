using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			return services;
		}
	}
}
