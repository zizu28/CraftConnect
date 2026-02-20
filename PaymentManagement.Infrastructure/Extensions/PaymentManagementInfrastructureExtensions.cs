using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Infrastructure.RepositoryImplementations;

namespace PaymentManagement.Infrastructure.Extensions
{
	public static class PaymentManagementInfrastructureExtensions
	{
		public static IServiceCollection AddPaymentManagementInfrastructureConfiguration(
			this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
			options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
				b => b.MigrationsAssembly(typeof(PaymentManagementInfrastructureExtensions).Assembly.FullName));
			});
			services.AddScoped<IInvoiceRepository, InvoiceRepository>();
			services.AddScoped<IPaymentRepository, PaymentRepository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
