namespace Core.SharedKernel.Domain
{
	public interface IDomainEventsDispatcher
	{
		Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
	}
}
