using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Contracts;
using UserManagement.Infrastructure.RepositoryImplementations;

namespace UserManagement.Infrastructure.Extensions
{
	public static class AddUserManagementInfrastructureConfiguration
	{
		public static IServiceCollection AddUserManagementConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ITokenProvider, TokenProvider>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<ITokenProvider, TokenProvider>();
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<ICraftsmanRepository, CraftmanRepository>();
			return services;
		}
	}
}
