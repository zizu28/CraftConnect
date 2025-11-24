using AuditManagement.Application.DTOs;
using MediatR;

namespace AuditManagement.Application.CQRS.Queries
{
	public class GetAllAuditLogsQuery : IRequest<List<AuditLogResponseDto>>
	{
	}
}
