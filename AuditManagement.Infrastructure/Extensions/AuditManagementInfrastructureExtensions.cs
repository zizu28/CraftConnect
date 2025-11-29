using AuditManagement.Application.Contracts;
using AuditManagement.Infrastructure.RepositoryImplementations;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace AuditManagement.Infrastructure.Extensions
{
	public static class AuditManagementInfrastructureExtensions
	{
		public static IServiceCollection AddAuditInfrastructureExtensions(this IServiceCollection services)
		{
			services.AddScoped<IAuditLogRepository, AuditLogRepository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			return services;
		}
	}
}
