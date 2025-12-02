namespace CraftConnect.WASM.ViewModels
{
	public class MessagesViewModel
	{
		public UserProfileVM CurrentUser { get; set; } = new();
		public List<ConversationSummaryVM> Conversations { get; set; } = [];
		public string ActiveProjectTitle { get; set; } = "";
		public string ActiveConversationWith { get; set; } = "";
		public List<ChatMessageDataVM> ActiveChatMessages { get; set; } = [];
	}
}
