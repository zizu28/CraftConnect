using Core.SharedKernel.Contracts;
using Core.SharedKernel.Domain;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
	public abstract class BaseRepositoryWithEvents<T> : BaseRepository<T> where T : AggregateRoot
	{
		protected BaseRepositoryWithEvents(ApplicationDbContext dbContext) : base(dbContext)
		{
		}

		public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			await base.AddAsync(entity, cancellationToken);
			await DispatchDomainEventsAsync(entity, cancellationToken);
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
			entity.ClearDomainEvents();

			foreach (var domainEvent in domainEvents)
			{
				// TODO: Integrate with MediatR or event dispatcher
				// await _mediator.Publish(domainEvent, cancellationToken);
			}
		}
	}
}