using MediatR;

namespace AuditManagement.Application.CQRS.Commands
{
	public class DeleteAuditLogCommand : IRequest
	{
		public Guid AuditLogId { get; set; }
	}
}
