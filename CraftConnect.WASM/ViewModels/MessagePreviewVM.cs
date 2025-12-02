namespace CraftConnect.WASM.ViewModels
{
	public class MessagePreviewVM
	{
		public string SenderName { get; set; } = string.Empty;
		public string Snippet { get; set; } = string.Empty;
		public string TimeAgo { get; set; } = string.Empty;
		public string AvatarUrl { get; set; } = string.Empty;
		public bool IsOnline { get; set; }
	}
}
