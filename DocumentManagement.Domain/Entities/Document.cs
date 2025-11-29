using Core.SharedKernel.Domain;
using Core.SharedKernel.DomainEvents;
using System.Drawing;

namespace DocumentManagement.Domain.Entities
{
	public class Document : AggregateRoot
	{
		public Guid StorageId { get; private set; }
		public Guid UploadedByUserId { get; private set; }
		public string FileName { get; private set; } = string.Empty;
		public string MimeType { get; private set; } = string.Empty;
		public long FileSizeInBytes { get; private set; }
		public DateTime Timestamp { get; private set; }

		private Document() { }

		public static Document Create(Guid storageId, string fileName, string mimeType, long size, Guid userId)
		{
			return new Document
			{
				StorageId = storageId,
				FileName = fileName,
				MimeType = mimeType,
				FileSizeInBytes = size,
				UploadedByUserId = userId
			};
		}

		public void Delete()
		{
			AddIntegrationEvent(new DocumentDeleted(Id, StorageId.ToString()));
		}
	}
}
