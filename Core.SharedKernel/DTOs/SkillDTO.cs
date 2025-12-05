namespace Core.SharedKernel.DTOs
{
	public record SkillDTO
	{
		public string Name { get; set; } = string.Empty;
		public int YearsOfExperience { get; set; }
	}
}
