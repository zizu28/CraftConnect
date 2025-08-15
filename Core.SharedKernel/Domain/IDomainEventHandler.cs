﻿namespace Core.SharedKernel.Domain
{
	public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
	{
		Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
	}
}
