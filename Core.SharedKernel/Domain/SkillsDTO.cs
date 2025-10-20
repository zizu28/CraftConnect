namespace Core.SharedKernel.Domain
{
	public class SkillsDTO
	{
		public string Name { get; set; } = string.Empty;
		public int YearsOfExperience { get; set; }

		public SkillsDTO(string name, int yearsOfExperience)
		{
			Name = name;
			YearsOfExperience = yearsOfExperience;
		}
	}
}
