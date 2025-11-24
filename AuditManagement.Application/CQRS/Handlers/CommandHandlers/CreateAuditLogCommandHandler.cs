using AuditManagement.Application.Contracts;
using AuditManagement.Application.CQRS.Commands;
using AuditManagement.Application.Validators;
using AuditManagement.Domain.Entities;
using AutoMapper;
using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;

namespace AuditManagement.Application.CQRS.Handlers.CommandHandlers
{
	public class CreateAuditLogCommandHandler(
		IAuditLogRepository auditLogRepository,
		ILoggingService<CreateAuditLogCommandHandler> logger,
		IMapper mapper,
		IUnitOfWork unitOfWork) : IRequestHandler<CreateAuditLogCommand>
	{
		public async Task Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
		{
			var validator = new CreateAuditLogDtoValidator();
			var validationResult = await validator.ValidateAsync(request.AuditLog, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Invalid audit log input data.");
				return;
			}
			var auditLog = mapper.Map<AuditLog>(request.AuditLog);
			await unitOfWork.ExecuteInTransactionAsync(
				async () => await auditLogRepository.AddAsync(auditLog), 
				cancellationToken
			);
			logger.LogInformation("Audit log with Id {LogId} added successfully.", auditLog.Id);
			return;
		}
	}
}
