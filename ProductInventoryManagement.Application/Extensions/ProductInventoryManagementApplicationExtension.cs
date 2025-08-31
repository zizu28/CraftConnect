using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductInventoryManagement.Application.Extensions
{
	public static class ProductInventoryManagementApplicationExtension
	{
		public static IServiceCollection AddProductInventoryManagementApplication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(typeof(ProductInventoryManagementApplicationExtension).Assembly);
				config.LicenseKey = configuration["MediatR:LicenseKey"];
			});
			services.AddAutoMapper(cfg =>
			{
				cfg.AddMaps(typeof(ProductInventoryManagementApplicationExtension).Assembly);
			});
			services.AddValidatorsFromAssemblyContaining(typeof(ProductInventoryManagementApplicationExtension));
			return services;
		}
	}
}
