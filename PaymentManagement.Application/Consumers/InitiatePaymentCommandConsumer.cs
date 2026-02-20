using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Commands.PaymentCommands;
using Core.SharedKernel.IntegrationEvents.PaymentsIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using MassTransit;
using MediatR;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;

namespace PaymentManagement.Application.Consumers
{
	/// <summary>
	/// Consumes InitiatePaymentCommand from SAGA to create and initiate payment
	/// </summary>
	public class InitiatePaymentCommandConsumer(
		IMediator mediator,
		IMessageBroker publishEndpoint,
		ILoggingService<InitiatePaymentCommandConsumer> logger) : IConsumer<InitiatePaymentCommand>
	{
		public async Task Consume(ConsumeContext<InitiatePaymentCommand> context)
		{
			var command = context.Message;
			logger.LogInformation("Initiating payment for booking {BookingId}, SAGA {CorrelationId}", 
				command.BookingId, command.CorrelationId);

			try
			{
				// Create payment using existing CreatePaymentCommand handler
				var createPaymentCommand = new CreatePaymentCommand
				{
					PaymentDTO = new()
					{
						Amount = command.Amount,
						Currency = command.Currency,
						PaymentMethod = "Card", // Assuming card payment for bookings
						BillingStreet = null, // Not needed for booking payments
						BillingCity = null,
						BillingPostalCode = null,
						Reference = $"BKG-{command.BookingId}-{Guid.NewGuid().ToString()[..8]}",
						PaymentStatus = "Pending",
						PaymentType = "Booking",
						PayerId = command.CustomerId,
						RecipientId = command.RecipientId, // Will be set by domain
						Email = command.CustomerEmail,
						CallbackUrl = command.CallbackUrl ?? $"/api/bookings/{command.BookingId}/payment-callback",
						Description = command.Description
					}
				};

				var paymentResponse = await mediator.Send(createPaymentCommand, context.CancellationToken);

				if (paymentResponse.IsSuccess && paymentResponse.Id != Guid.Empty)
				{
					// Publish success event back to SAGA
					await publishEndpoint.PublishAsync(new PaymentInitiatedIntegrationEvent(
						paymentResponse.Id,
						null, // No invoice for booking payments
						new Money(command.Amount, command.Currency),
						command.CustomerId,
						command.RecipientId), context.CancellationToken);

					logger.LogInformation("Payment with ID {PaymentId} initiated successfully for booking with ID {BookingId}", 
						paymentResponse.Id, command.BookingId);
				}
				else
				{
					// Publish failure event
					logger.LogError(new Exception(), "Failed to initiate payment for booking with ID {BookingId}: {Errors}", 
						command.BookingId, string.Join(", ", paymentResponse.Errors ?? []));

					await publishEndpoint.PublishAsync(new PaymentFailedIntegrationEvent(
						Guid.Empty,
						command.BookingId,
						null,
						null,
						paymentResponse.Message ?? "Payment initiation failed",
						command.CustomerId,
						command.RecipientId), context.CancellationToken);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error initiating payment for booking {BookingId}", command.BookingId);
				
				// Publish failure event
				await publishEndpoint.PublishAsync(new PaymentFailedIntegrationEvent(
					Guid.Empty,
					command.BookingId,
					null,
					null,
					$"Exception: {ex.Message}",
					command.CustomerId,
					command.RecipientId), context.CancellationToken);
			}
		}
	}
}
