using CraftConnect.WebUI.Enums;

namespace CraftConnect.WebUI.ViewModels
{
	public class SignupVM
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
		public bool AgreeToTerms { get; set; }
		public string Role { get; set; }
	}
}
