using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.IntegrationEvents.NotificationIntegrationEvents
{
	public record NotificationFailedIntegrationEvent(
	Guid NotificationId,
	Guid RecipientId,
	NotificationType Type,
	string ErrorMessage,
	DateTime FailedAt) : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
	}
}
