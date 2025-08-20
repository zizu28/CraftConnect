using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.IntegrationEvents
{
	public record CraftmanVerifiedIntegrationEvent(Guid CraftmanId, DocumentType DocumentType) : IIntegrationEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid EventId => Guid.NewGuid();
	}
}
