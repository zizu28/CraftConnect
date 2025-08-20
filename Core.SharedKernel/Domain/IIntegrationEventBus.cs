namespace Core.SharedKernel.Domain
{
	public interface IIntegrationEventBus
	{
		Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
	}
}
