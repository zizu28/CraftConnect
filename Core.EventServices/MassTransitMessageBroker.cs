using Core.SharedKernel.Domain;
using MassTransit;

namespace Core.EventServices
{
	public class MassTransitMessageBroker(
		IPublishEndpoint publish, ISendEndpointProvider send) : IMessageBroker
	{
		private readonly IPublishEndpoint _publish = publish;
		private readonly ISendEndpointProvider _send = send;

		public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IIntegrationEvent
		{
			await _publish.Publish(message, message.GetType(),cancellationToken);
		}

		public async Task SendAsync<T>(string queueName, T command, CancellationToken cancellationToken = default) where T : IIntegrationEvent
		{
			var endpoint = await _send.GetSendEndpoint(new Uri($"queue:{queueName}"));
			await endpoint.Send(command, command.GetType(),cancellationToken);
		}
	}
}
