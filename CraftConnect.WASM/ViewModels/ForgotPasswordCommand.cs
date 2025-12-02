using System.ComponentModel.DataAnnotations;

namespace CraftConnect.WASM.ViewModels
{
	public class ForgotPasswordCommand
	{
		[Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
		public string Email { get; set; }
	}
}
