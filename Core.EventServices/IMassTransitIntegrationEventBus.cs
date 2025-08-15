using Core.SharedKernel.Domain;

namespace Core.EventServices
{
	public interface IMassTransitIntegrationEventBus
	{
		Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IIntegrationEvent;
	}
}
