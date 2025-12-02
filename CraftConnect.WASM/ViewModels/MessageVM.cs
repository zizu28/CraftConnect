namespace CraftConnect.WASM.ViewModels
{
	public class MessageVM
	{
		public Guid Id { get; set; }
		public string AuthorName { get; set; }
		public string Content { get; set; }
		public string AuthorInitials { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsYou { get; set; }
	}
}
