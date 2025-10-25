namespace CraftConnect.WebUI.ViewModels
{
	public class UserProfileVM
	{
		public string Name { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty; // e.g., Customer, Artisan
		public string AvatarUrl { get; set; } = string.Empty;
		public bool IsCustomer { get; set; }
		public Guid Id { get; set; }
		public string AvatarInitials { get; set; } = "ZZ";
	}
}
