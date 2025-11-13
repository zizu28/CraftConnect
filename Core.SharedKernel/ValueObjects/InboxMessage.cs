namespace Core.SharedKernel.ValueObjects
{
	public record InboxMessage
	{
		public int Id { get; set; }
		public string SenderName { get; set; } = string.Empty;
		public string SenderAvatarInitials { get; set; } = string.Empty;
		public string Excerpt { get; set; } = string.Empty;
		public string ReceivedTime { get; set; } = string.Empty;
		public bool IsRead { get; set; }
		public bool IsSupportTicket { get; set; }
	}
}
