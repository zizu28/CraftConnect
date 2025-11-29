using Core.SharedKernel.Domain;
using System.Net;

namespace AuditManagement.Domain.Entities
{
	public class AuditLog : AggregateRoot
	{
		public Guid? UserId { get; private set; }
		public Guid? EntityId { get; private set; }
		public DateTime Timestamp { get; private set; }
		public string EventType { get; private set; } = string.Empty;
		public string Details { get; private set; } = string.Empty;
		public IPAddress IpAddress { get; private set; }
		public string OldValue { get; private set; } = string.Empty;
		public string NewValue { get; private set; } = string.Empty;

		private AuditLog() {	}

		public static AuditLog Create(string eventType, Guid userId, string details, 
			string ipAddress, Guid entityId, string oldValue, string newValue)
		{
			ArgumentException.ThrowIfNullOrEmpty(eventType);
			ArgumentException.ThrowIfNullOrEmpty(details);
			ArgumentException.ThrowIfNullOrEmpty(ipAddress);
			ArgumentException.ThrowIfNullOrEmpty(oldValue);
			ArgumentException.ThrowIfNullOrEmpty(newValue);
			if (userId == Guid.Empty || entityId == Guid.Empty) 
				throw new ArgumentException("UserId or EntityId cannot be null.");
			return new AuditLog
			{
				EventType = eventType,
				UserId = userId,
				EntityId = entityId,
				OldValue = oldValue,
				IpAddress = IPAddress.Parse(ipAddress),
				NewValue = newValue,
				Details = details
			};
		}
	}
}
