namespace Core.SharedKernel.Domain
{
	public abstract class AggregateRoot : Entity
	{
		private	readonly List<IDomainEvent>	 _domainEvents = [];
		public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

		protected void AddDomainEvent(IDomainEvent domainEvent)
		{
			_domainEvents.Add(domainEvent);
		}

		public void ClearEvents()
		{
			_domainEvents.Clear();
		}
	}
}
