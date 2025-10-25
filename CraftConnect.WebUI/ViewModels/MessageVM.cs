namespace CraftConnect.WebUI.ViewModels
{
	public class MessageVM
	{
		public int Id { get; set; }
		public string AuthorName { get; set; }
		public string AuthorInitials { get; set; }
		public string Content { get; set; }
		public bool IsYou { get; set; }
		public string Text { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; }
		public bool IsSentByCurrentUser { get; set; }
	}
}
