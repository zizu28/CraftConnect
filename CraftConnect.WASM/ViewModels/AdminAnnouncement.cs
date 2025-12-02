using CraftConnect.WASM.Enums;

namespace CraftConnect.WASM.ViewModels
{
	public class AdminAnnouncement
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public DateTime PublicationDate { get; set; }
		public AnnouncementStatus Status { get; set; }
		public string TargetAudience { get; set; }
	}
}
