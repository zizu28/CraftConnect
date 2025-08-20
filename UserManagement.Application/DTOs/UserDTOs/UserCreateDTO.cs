namespace UserManagement.Application.DTOs.UserDTOs
{
	public class UserCreateDTO
	{
		public required string Username { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string PhoneCountryCode { get; set; }
		public required string PhoneNumber { get; set; }
		public required string Role { get; set; }
	}
}
