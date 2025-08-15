namespace UserManagement.Application.DTOs.CraftmanDTO
{
	public class CraftmanCreateDTO
	{
		public required string UserName { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string Profession { get; set; }
		public required string Bio { get; set; }
		public required string Status { get; set; }
		public int YearsOfExperience { get; set; }
		public decimal HourlyRate { get; set; }
		public required string Currency { get; set; }
		public required string PhoneCountryCode { get; set; }
		public required string PhoneNumber { get; set; }
		public List<string> Skills { get; set; } = [];
	}
}
