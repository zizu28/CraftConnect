using FluentValidation;
using Infrastructure.Persistence.Data;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace UserManagement.Application.Extensions
{
	public static class UserManagementApplicationExtensions
	{
		public static IServiceCollection AddUserApplicationExtensions(
			this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMassTransit(mt =>
			{
				mt.AddEntityFrameworkOutbox<ApplicationDbContext>(config =>
				{
					config.QueryDelay = TimeSpan.FromSeconds(30);
					config.UsePostgres().UseBusOutbox();
				});
				mt.SetKebabCaseEndpointNameFormatter();
				mt.AddConsumers(typeof(UserManagementApplicationExtensions).Assembly);

			});

			services.AddHttpContextAccessor();
			services.AddMediatR(config =>
			{
				config.RegisterServicesFromAssembly(typeof(UserManagementApplicationExtensions).Assembly);
				config.LicenseKey = configuration["MediatR:LicenseKey"];
			});
			services.AddAutoMapper(cfg =>
			{
				cfg.AddMaps(typeof(UserManagementApplicationExtensions).Assembly);
			});
			services.AddValidatorsFromAssemblyContaining(typeof(UserManagementApplicationExtensions));
			return services;
		}
	}
}
