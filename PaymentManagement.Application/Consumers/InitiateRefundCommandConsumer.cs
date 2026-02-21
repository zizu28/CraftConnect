using Core.Logging;
using Core.SharedKernel.Commands.PaymentCommands;
using MassTransit;
using MediatR;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;

namespace PaymentManagement.Application.Consumers
{
	/// <summary>
	/// Consumes InitiateRefundCommand from SAGA to refund payment when booking confirmation fails
	/// </summary>
	public class InitiateRefundCommandConsumer(
		IMediator mediator,
		ILoggingService<InitiateRefundCommandConsumer> logger) : IConsumer<InitiateRefundCommand>
	{
		public async Task Consume(ConsumeContext<InitiateRefundCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Initiating refund for payment {PaymentId}, SAGA {CorrelationId}. Reason: {Reason}", 
				command.PaymentId, command.CorrelationId, command.Reason);

			try
			{
				// Use existing RefundPaymentCommand handler
				var refundCommand = new RefundPaymentCommand
				{
					PaymentId = command.PaymentId,
					RefundAmount = command.Amount,
					Reason = command.Reason,
					RecipientEmail = command.RecipientEmail,
					Currency = command.Currency,
					InitiatedBy = Guid.Empty // System initiated, no user
				};

				var refundResponse = await mediator.Send(refundCommand, context.CancellationToken);
				logger.LogInformation("Refund initiated successfully for payment {PaymentId}", command.PaymentId);

				if (refundResponse)
				{
					logger.LogInformation("Refund initiated successfully for payment {PaymentId}", command.PaymentId);
				}
				else
				{
					logger.LogError(new Exception(), "Failed to initiate refund for payment with ID {PaymentId}", command.PaymentId);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error initiating refund for payment {PaymentId}", command.PaymentId);
				// Swallow exception - SAGA will continue with cancellation
				// Manual refund may be required
			}
		}
	}
}
