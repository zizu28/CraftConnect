using Core.SharedKernel.Domain;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repository
{
	public abstract class BaseRepositoryWithEvents<T>(ApplicationDbContext dbContext) : BaseRepository<T>(dbContext) where T : AggregateRoot
	{
		public override async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			await base.AddAsync(entity, cancellationToken);
			await DispatchDomainEventsAsync(entity, cancellationToken);
			return entity;
		}

		public override async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
		{
			await base.UpdateAsync(entity, cancellationToken);
			await DispatchDomainEventsAsync(entity, cancellationToken);
		}

		private async Task DispatchDomainEventsAsync(T entity, CancellationToken cancellationToken)
		{
			var domainEvents = entity.DomainEvents.ToList();
			
			// Clear events to prevent re-processing
			entity.ClearEvents();

			foreach (var domainEvent in domainEvents)
			{
				// TODO: Integrate with MediatR or event dispatcher
				// await _mediator.Publish(domainEvent, cancellationToken);
			}
		}
	}
}