namespace Core.SharedKernel.Domain
{
	public interface IDomainEventsDispatcher
	{
		Task DispatchAsync(IEnumerable<IIntegrationEvent> domainEvents, CancellationToken cancellationToken = default);
	}
}
