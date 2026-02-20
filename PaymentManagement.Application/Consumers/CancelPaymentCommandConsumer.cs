using Core.Logging;
using Core.SharedKernel.Commands.PaymentCommands;
using Core.SharedKernel.Enums;
using Infrastructure.Persistence.UnitOfWork;
using MassTransit;
using PaymentManagement.Application.Contracts;

namespace PaymentManagement.Application.Consumers
{
	/// <summary>
	/// Consumes CancelPaymentCommand from SAGA to cancel a timed-out payment
	/// </summary>
	public class CancelPaymentCommandConsumer(
		IPaymentRepository paymentRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<CancelPaymentCommandConsumer> logger) : IConsumer<CancelPaymentCommand>
	{
		public async Task Consume(ConsumeContext<CancelPaymentCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Cancelling payment {PaymentId}. Reason: {Reason}", 
				command.PaymentId, command.Reason);

			try
			{
				// Get the payment
				var payment = await paymentRepository.GetByIdAsync(command.PaymentId, context.CancellationToken);
				if (payment == null)
				{
					logger.LogWarning("Payment {PaymentId} not found for cancellation", command.PaymentId);
					return; // Idempotent - already cancelled or doesn't exist
				}

				// Only cancel if still in pending state
				if (payment.Status == PaymentStatus.Pending)
				{
					payment.Cancel(command.Reason);
					await unitOfWork.ExecuteInTransactionAsync(async () =>
					{
						await paymentRepository.UpdateAsync(payment, context.CancellationToken);
						logger.LogInformation("Payment with ID {PaymentId} cancelled successfully", command.PaymentId);
					}, context.CancellationToken);
				}
				else
				{
					logger.LogWarning("Payment with ID {PaymentId} is in status {Status}, cannot cancel", 
						command.PaymentId, payment.Status);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error cancelling payment {PaymentId}", command.PaymentId);
			}
		}
	}
}
