namespace CraftConnect.WebUI.ViewModels
{
	public class ConversationPreviewVM
	{
		public string PartnerName { get; set; } = string.Empty;
		public string LastMessageSnippet { get; set; } = string.Empty;
		public string PartnerAvatarUrl { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public bool IsOnline { get; set; }
	}
}
