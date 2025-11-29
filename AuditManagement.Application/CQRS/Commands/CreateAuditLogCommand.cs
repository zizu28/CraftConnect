using AuditManagement.Application.DTOs;
using MediatR;

namespace AuditManagement.Application.CQRS.Commands
{
	public class CreateAuditLogCommand : IRequest
	{
		public CreateAuditLogDto AuditLog { get; set; }
	}
}
