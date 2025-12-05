namespace BookingManagement.Application.DTOs.Shared
{
	public record SkillDTO
	{
		public string Name { get; set; } = string.Empty;
		public int YearsOfExperience { get; set; }
	}
}
