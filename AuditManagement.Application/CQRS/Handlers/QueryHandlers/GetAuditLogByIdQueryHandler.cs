using AuditManagement.Application.Contracts;
using AuditManagement.Application.CQRS.Queries;
using AutoMapper;
using Core.SharedKernel.DTOs;
using MediatR;

namespace AuditManagement.Application.CQRS.Handlers.QueryHandlers
{
	public class GetAuditLogByIdQueryHandler(
		IAuditLogRepository auditLogRepository,
		IMapper mapper) : IRequestHandler<GetAuditLogByIdQuery, AuditLogResponseDto>
	{
		public async Task<AuditLogResponseDto> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
		{
			var auditLog = await auditLogRepository.GetByIdAsync(request.AuditLogId, cancellationToken)
				?? throw new KeyNotFoundException($"Audit log with ID {request.AuditLogId} not found");
			return mapper.Map<AuditLogResponseDto>(auditLog);
		}
	}
}
