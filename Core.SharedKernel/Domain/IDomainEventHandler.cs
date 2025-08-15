namespace Core.SharedKernel.Domain
{
	public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IIntegrationEvent
	{
		Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
	}
}
