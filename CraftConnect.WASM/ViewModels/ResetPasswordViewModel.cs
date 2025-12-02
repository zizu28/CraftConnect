using System.ComponentModel.DataAnnotations;

namespace CraftConnect.WASM.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "New password is required")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
			ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Please confirm your new password")]
		[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
		public string ConfirmNewPassword { get; set; }
	}
}
