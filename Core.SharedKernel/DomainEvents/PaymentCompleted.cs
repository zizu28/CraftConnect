using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record PaymentCompleted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
