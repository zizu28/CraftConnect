using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.DTOs.CraftmanDTO
{
	public class CraftmanCreateDTO : UserCreateDTO
	{
		public string Profession { get; set; } = string.Empty;
		public string Bio { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public int YearsOfExperience { get; set; } 
		public decimal HourlyRate { get; set; } 
		public string Currency { get; set; } = string.Empty;
		public List<string> Skills { get; set; } = [];
	}
}
