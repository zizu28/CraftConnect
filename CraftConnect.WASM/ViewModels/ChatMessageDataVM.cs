namespace CraftConnect.WASM.ViewModels
{
	public class ChatMessageDataVM
	{
		public Guid Id { get; set; }
		public string SenderName { get; set; }
		public string SenderAvatarUrl { get; set; }
		public string MessageText { get; set; }
		public string Timestamp { get; set; }
		public bool IsSentByUser { get; set; } // True if "John Smith" sent it
		public AttachedDocument? Attachment { get; set; } // Optional attachment
		public bool IsSeen { get; set; }
	}
}
