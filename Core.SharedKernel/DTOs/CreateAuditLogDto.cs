namespace Core.SharedKernel.DTOs
{
	public class CreateAuditLogDto
	{
		public string EventType { get; set; } = string.Empty;
		public Guid UserId { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Details { get; set; } = string.Empty;
		public string IpAddress { get; set; } = string.Empty;
		public Guid EntityId { get; set; }
		public string? EntityType { get; set; } // e.g., "Project", "User"
		public string OldValue { get; set; } = string.Empty; // JSON string
		public string NewValue { get; set; } = string.Empty;
	}
}
