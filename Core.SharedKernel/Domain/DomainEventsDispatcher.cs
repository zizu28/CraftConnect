using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Core.SharedKernel.Domain
{
	public class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventsDispatcher
	{
		private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
		private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();
		public async Task DispatchAsync(IEnumerable<IIntegrationEvent> domainEvents, CancellationToken cancellationToken = default)
		{
			foreach(var domainEvent in domainEvents)
			{
				using IServiceScope scope = serviceProvider.CreateScope();
				Type eventType = domainEvent.GetType();
				Type handlerType = HandlerTypeDictionary.GetOrAdd(
					eventType,
					et => typeof(IDomainEventHandler<>).MakeGenericType(et));
				IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(handlerType);
				foreach(var handler in handlers)
				{
					if (handler is null) continue;
					HandlerWrapper wrapper = HandlerWrapper.Create(handler, eventType);
					await wrapper.HandleAsync(domainEvent, cancellationToken);
				}
			}
		}

		private abstract class HandlerWrapper
		{
			public abstract Task HandleAsync(IIntegrationEvent domainEvent, CancellationToken cancellationToken);
			public static HandlerWrapper Create(object handler, Type domainEventType)
			{
				Type wrapperType = WrapperTypeDictionary.GetOrAdd(
					domainEventType,
					et => typeof(HandlerWrapper<>).MakeGenericType(et));
				return (HandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
			}
		}

		private class HandlerWrapper<T>(object handler) : HandlerWrapper where T : IIntegrationEvent
		{
			private readonly IDomainEventHandler<T> _handler = (IDomainEventHandler<T>)handler;

			public override async Task HandleAsync(IIntegrationEvent domainEvent, CancellationToken cancellationToken)
			{
				await _handler.HandleAsync((T)domainEvent, cancellationToken);
			}
		}
	}
}
