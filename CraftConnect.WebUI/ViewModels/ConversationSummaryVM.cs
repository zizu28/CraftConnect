namespace CraftConnect.WebUI.ViewModels
{
	public class ConversationSummaryVM
	{
		public Guid Id { get; set; }
		public string ProjectTitle { get; set; }
		public string OtherUserName { get; set; }
		public string OtherUserAvatarUrl { get; set; }
		public string LatestMessagePreview { get; set; }
		public string LatestMessageTimestamp { get; set; } // e.g., "2:45 PM" or "Yesterday"
		public bool IsRead { get; set; }
	}
}
