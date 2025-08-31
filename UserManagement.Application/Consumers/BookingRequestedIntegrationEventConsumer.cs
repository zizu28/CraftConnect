using Core.Logging;
using Core.SharedKernel.IntegrationEvents;
using MassTransit;
using UserManagement.Application.Contracts;

namespace UserManagement.Application.Consumers
{
	public class BookingRequestedIntegrationEventConsumer(
		IUserRepository userRepository,
		ILoggingService<BookingRequestedIntegrationEventConsumer> logger) : IConsumer<BookingRequestedIntegrationEvent>
	{
		public async Task Consume(ConsumeContext<BookingRequestedIntegrationEvent> context)
		{
			var message = context.Message;
			logger.LogInformation("Received BookingRequestedIntegrationEvent for Booking ID: {BookingId}", message.BookingId);
			var craftman = await userRepository.GetByIdAsync(message.CraftspersonId, context.CancellationToken)
				?? throw new InvalidOperationException($"Craftsman with ID {message.CraftspersonId} not found.");
		}
	}
}
