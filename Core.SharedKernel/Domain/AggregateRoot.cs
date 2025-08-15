namespace Core.SharedKernel.Domain
{
	public abstract class AggregateRoot : Entity
	{
		private	readonly List<IIntegrationEvent>	 _domainEvents = [];
		public IReadOnlyCollection<IIntegrationEvent> DomainEvents => _domainEvents.AsReadOnly();

		protected void AddIntegrationEvent(IIntegrationEvent domainEvent)
		{
			_domainEvents.Add(domainEvent);
		}

		public void ClearEvents()
		{
			_domainEvents.Clear();
		}
	}
}
