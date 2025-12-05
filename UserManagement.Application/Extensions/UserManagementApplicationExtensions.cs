using Core.SharedKernel.Contracts;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Services;
namespace UserManagement.Application.Extensions
{
	public static class UserManagementApplicationExtensions
	{
		public static IServiceCollection AddUserApplicationExtensions(
			this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddMassTransit(mt =>
			//{
			//	mt.AddConsumersFromNamespaceContaining(typeof(UserManagementApplicationExtensions));

			//});

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
			services.AddScoped<IUserModuleService, UserModuleService>();
			return services;
		}
	}
}
