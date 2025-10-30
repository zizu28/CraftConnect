namespace UserManagement.Application.DTOs.UserDTOs
{
	public class UserCreateDTO
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string Role { get; set; }
		public required string ConfirmPassword { get; set; }
		public required bool AgreeToTerms { get; set; }
	}
}
