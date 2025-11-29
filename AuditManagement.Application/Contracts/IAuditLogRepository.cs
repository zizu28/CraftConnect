using AuditManagement.Domain.Entities;

namespace AuditManagement.Application.Contracts
{
	public interface IAuditLogRepository : IRepository<AuditLog>
	{
	}
}
