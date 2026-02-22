using Core.Logging;
using Core.SharedKernel.Commands.PaymentCommands;
using MassTransit;
using MediatR;

namespace PaymentManagement.Application.Consumers
{
	/// <summary>
	/// Consumes CancelPaymentCommand from SAGA to cancel a timed-out payment.
	/// Delegates all domain logic to <see cref="PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers.CancelPaymentCommandHandler"/>.
	/// </summary>
	public class CancelPaymentCommandConsumer(
		IMediator mediator,
		ILoggingService<CancelPaymentCommandConsumer> logger) : IConsumer<CancelPaymentCommand>
	{
		public async Task Consume(ConsumeContext<CancelPaymentCommand> context)
		{
			var message = context.Message;
			logger.LogInformation("Cancelling payment {PaymentId}. Reason: {Reason}",
				message.PaymentId, message.Reason);

			try
			{
				var appCommand = new CQRS.Commands.PaymentCommands.CancelPaymentCommand
				{
					CorrelationId = message.CorrelationId,
					PaymentId = message.PaymentId,
					Reason = message.Reason
				};

				await mediator.Send(appCommand, context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error cancelling payment {PaymentId}", message.PaymentId);
				// Rethrow so MassTransit can retry / move to error queue.
				throw;
			}
		}
	}
}
