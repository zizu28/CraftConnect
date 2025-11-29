using AuditManagement.Application.DTOs;
using MediatR;

namespace AuditManagement.Application.CQRS.Queries
{
	public class GetAuditLogByIdQuery : IRequest<AuditLogResponseDto>
	{
		public Guid AuditLogId { get; set; }
	}
}
