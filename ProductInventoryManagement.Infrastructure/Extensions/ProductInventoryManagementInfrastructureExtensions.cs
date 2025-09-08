using Infrastructure.Persistence.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Infrastructure.RepositoryImplementations;

namespace ProductInventoryManagement.Infrastructure.Extensions
{
	public static class ProductInventoryManagementInfrastructureExtensions
	{
		public static IServiceCollection ProductInventoryManagementInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddMassTransit(mt =>
			//{
			//	mt.AddConsumersFromNamespaceContaining(typeof(ProductInventoryManagementInfrastructureExtensions));
			//});

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					configuration.GetConnectionString("DefaultConnection"),
					b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<ICategoryRepository, CategoryRepository>();
			return services;
		}
	}
}
