using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using Core.SharedKernel.Enums;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class CancelPaymentCommandHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<CancelPaymentCommandHandler> logger,
		IUnitOfWork unitOfWork) : IRequestHandler<CancelPaymentCommand, Unit>
	{
		public async Task<Unit> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
		{
			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

			if (payment == null)
			{
				logger.LogWarning("Payment {PaymentId} not found for cancellation — already cancelled or does not exist.", request.PaymentId);
				return Unit.Value; // Idempotent
			}

			if (payment.Status != PaymentStatus.Pending)
			{
				logger.LogWarning("Payment {PaymentId} is in status {Status} and cannot be cancelled.",
					request.PaymentId, payment.Status);
				return Unit.Value;
			}

			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				payment.Cancel(request.CorrelationId, request.Reason);
				await paymentRepository.UpdateAsync(payment, cancellationToken);
				logger.LogInformation("Payment {PaymentId} cancelled successfully. Reason: {Reason}",
					request.PaymentId, request.Reason);
			}, cancellationToken);

			return Unit.Value;
		}
	}
}
