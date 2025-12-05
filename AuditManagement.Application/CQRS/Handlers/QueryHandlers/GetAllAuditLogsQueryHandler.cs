using AuditManagement.Application.Contracts;
using AuditManagement.Application.CQRS.Queries;
using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;

namespace AuditManagement.Application.CQRS.Handlers.QueryHandlers
{
	public class GetAllAuditLogsQueryHandler(
		IAuditLogRepository auditLogRepository,
		IMapper mapper) : IRequestHandler<GetAllAuditLogsQuery, List<AuditLogResponseDto>>
	{
		public async Task<List<AuditLogResponseDto>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
		{
			var auditLogs = await auditLogRepository.GetAllAsync(cancellationToken)
				?? throw new ArgumentNullException(nameof(request), $"Could not retrieve list of bookings from database or cache.");
			return mapper.Map<List<AuditLogResponseDto>>(auditLogs);
		}
	}
}
