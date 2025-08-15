using Core.SharedKernel.Domain;

namespace Core.EventServices
{
	public interface IMessageBroker
	{
		Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IIntegrationEvent;
		Task SendAsync<T>(string queueName, T command, CancellationToken cancellationToken = default) where T : IDomainEvent;
		//Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> handler) where T : class;
	}
}
