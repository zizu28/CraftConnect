namespace Core.SharedKernel.DTOs
{
	public class AuditLogResponseDto
	{
		public Guid Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string EventType { get; set; } = string.Empty;
		public Guid? UserId { get; set; }
		public string Username { get; set; } = string.Empty; // Display name
		public string Details { get; set; } = string.Empty;
		public string IpAddress { get; set; } = string.Empty;
		public Guid? EntityId { get; set; }
		public string? EntityType { get; set; }
		public string? OldValue { get; set; }
		public string? NewValue { get; set; }
	}
}
