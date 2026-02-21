using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents
{
	public record NotificationSentIntegrationEvent(
		Guid CorrelationId,
		Guid NotificationId,
		Guid RecipientId,
		NotificationType Type,
		NotificationChannel Channel,
		DateTime SentAt) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
