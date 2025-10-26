using CraftConnect.WebUI.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class SignupVM
	{
		public string EmailAddress { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
		public bool AgreeToTerms { get; set; }
		public UserType SelectedUserType { get; set; } = UserType.Craftperson;
	}
}
