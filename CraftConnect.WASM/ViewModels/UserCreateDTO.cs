namespace CraftConnect.WASM.ViewModels
{
	public class UserCreateDTO
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
		public string ConfirmPassword { get; set; }
		public bool AgreeToTerms { get; set; }
	}
}
