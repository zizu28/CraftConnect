using Core.SharedKernel.Domain;
using MassTransit;

namespace Core.EventServices
{
	public class MassTransitIntegrationEventBus(IPublishEndpoint publishEndpoint) : IMassTransitIntegrationEventBus
	{
		private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

		public Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : class, IIntegrationEvent
		{
			return _publishEndpoint.Publish(integrationEvent, integrationEvent.GetType(), cancellationToken);
		}
	}
}
