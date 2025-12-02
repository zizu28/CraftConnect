namespace CraftConnect.WASM.ViewModels
{
	public class LoginUserCommand
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string RedirectUri { get; set; } = string.Empty;
		public bool RememberMe { get; set; }
	}
}
