using Core.SharedKernel.Domain;

namespace Core.SharedKernel.DomainEvents
{
	public record DocumentDeleted : IIntegrationEvent
	{
		public Guid EventId => Guid.NewGuid();
		public DateTime OccuredOn => DateTime.UtcNow;
		public Guid DocumentId { get; }
		public string StorageId { get; } // The path/key in S3/Blob storage

		public DocumentDeleted(Guid documentId, string storageId)
		{
			DocumentId = documentId;
			StorageId = storageId;
		}
	}
}
