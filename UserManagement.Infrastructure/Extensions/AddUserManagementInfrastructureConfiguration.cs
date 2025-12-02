using Infrastructure.Persistence.UnitOfWork;
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
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<ICraftsmanRepository, CraftmanRepository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddKeyedScoped<IFileStorageService, LocalFileStorageService>("localStorage");
			services.AddKeyedScoped<IFileStorageService, S3FileStorageService>("S3Storage");
			services.AddKeyedScoped<IFileStorageService, CloudinaryStorageService>("Cloudinary");
			var useCloudinary = configuration.GetValue<bool>("Storage:UseCloudinary");

			if (useCloudinary)
			{
				services.AddScoped<IFileStorageService, CloudinaryStorageService>();
			}
			else
			{
				services.AddScoped<IFileStorageService, LocalFileStorageService>();
			}
			return services;
		}
	}
}
