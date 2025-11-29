using CraftConnect.WebUI.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class AdminUser
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string AvatarInitial { get; set; }
		public UserType Role { get; set; }
		public UserStatus Status { get; set; }
		public DateTime DateJoined { get; set; }
		public DateTime LastActive { get; set; }
	}
}
