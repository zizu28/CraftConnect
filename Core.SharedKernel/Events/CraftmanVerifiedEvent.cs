using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;

namespace Core.SharedKernel.Events
{
	public record CraftmanVerifiedEvent(Guid CraftmanId, DocumentType DocumentType) : IDomainEvent
	{
		public DateTime OccuredOn => DateTime.UtcNow;

		public Guid Id => Guid.NewGuid();
	}
}
