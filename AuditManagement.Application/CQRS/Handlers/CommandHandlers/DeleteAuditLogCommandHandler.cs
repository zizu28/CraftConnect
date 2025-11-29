using AuditManagement.Application.Contracts;
using AuditManagement.Application.CQRS.Commands;
using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace AuditManagement.Application.CQRS.Handlers.CommandHandlers
{
	public class DeleteAuditLogCommandHandler(
		IAuditLogRepository auditLogRepository,
		ILoggingService<CreateAuditLogCommandHandler> logger,
		IUnitOfWork unitOfWork) : IRequestHandler<DeleteAuditLogCommand>
	{
		public async Task Handle(DeleteAuditLogCommand request, CancellationToken cancellationToken)
		{
			var auditLog = await auditLogRepository.GetByIdAsync(request.AuditLogId, cancellationToken)
				?? throw new KeyNotFoundException($"Audit log with Id {request.AuditLogId} not found.");
			await unitOfWork.ExecuteInTransactionAsync(
				async () => await auditLogRepository.DeleteAsync(auditLog.Id, cancellationToken),
				cancellationToken
			);
			logger.LogInformation("Audit log with Id {LogId} removed successfully", auditLog.Id);
		}
	}
}
